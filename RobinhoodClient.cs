using Newtonsoft.Json;
using Penguin.Collections;
using Penguin.Debugging;
using Penguin.Robinhood.Exceptions;
using Penguin.Robinhood.Extensions;
using Penguin.Robinhood.Interfaces;
using Penguin.Robinhood.Models;
using Penguin.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace Penguin.Robinhood
{
    public class RobinhoodClient : JsonClient, IDisposable
    {
        public static readonly DictionaryFile Tickers = new(Path.Combine(AppData, "Tickers.Cache"));

        public readonly LogWriter LogWriter;

        public string Authority = "https://api.robinhood.com";

        private const string AUTHORIZATION_HEADER = "Authorization";

        private AuthenticationResponse Authentication;

        private readonly IAuthenticationSettings Settings;

        public static string AppData
        {
            get
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Robinhood");

                if (!Directory.Exists(directory))
                {
                    _ = Directory.CreateDirectory(directory);
                }

                return directory;
            }
        }

        public IReadOnlyList<Account> Accounts { get; private set; }

        public static string AccountsUrl => $"https://nummus.robinhood.com/accounts/";

        public string AuthUrl => $"{Authority}/oauth2/token/";

        public static string HoldingUrl => $"https://nummus.robinhood.com/holdings/";

        public static string OrderUrl => $"https://nummus.robinhood.com/orders/";

        public string QuotesUrl => $"{Authority}/marketdata/forex/quotes/";

        public static string UnifiedAccountUrl => $"https://phoenix.robinhood.com/accounts/unified";

        public UserResponse User { get; private set; }

        public string UserUrl => $"{Authority}/user/";

        private static string LogDirectory => Path.Combine(AppData, "Logs", $"{Process.GetCurrentProcess().ProcessName}");

        private static string CachedAuthPath => Path.Combine(AppData, "Token.json");

        public RobinhoodClient(IAuthenticationSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Settings = settings;

            LogWriter = new LogWriter(new LogWriterSettings()
            {
                ObjectSerializationOverride = JsonConvert.SerializeObject,
                OutputTarget = LogOutput.Debug | LogOutput.File,
                Directory = LogDirectory
            });

            AuthenticationResponse existingAuth = null;

            if (File.Exists(CachedAuthPath))
            {
                existingAuth = JsonConvert.DeserializeObject<AuthenticationResponse>(File.ReadAllText(CachedAuthPath));

                if (existingAuth.IsExpired)
                {
                    existingAuth = null;
                }
            }

            if (existingAuth is null)
            {
                RefreshSession();
            }
            else
            {
                Authentication = existingAuth;
            }

            User = DownloadJson<UserResponse>(UserUrl);

            Accounts = DownloadJson<AccountsResponse>(AccountsUrl).Results;
        }

        public static void CacheDataPoint(Guid id, DataPoint dp)
        {
            IHasId ihid = new SymbolResponse()
            {
                Id = id
            };

            CacheDataPoint(ihid, dp);
        }

        public static void CacheDataPoint(IHasId id, DataPoint dp)
        {
            if (dp is null)
            {
                throw new ArgumentNullException(nameof(dp));
            }

            string datapointcache = $"{id.DataPointDirectory()}.json";

            using StreamWriter sw = new(datapointcache, true);

            sw.WriteLine(dp.ToString());
        }

        public static string FindSymbol(IHasId id)
        {
            return id is null ? throw new ArgumentNullException(nameof(id)) : FindSymbol(id.Id);
        }

        public static string FindSymbol(Guid id)
        {
            foreach (KeyValuePair<string, string> kvp in Tickers)
            {
                if (kvp.Value == $"{id}")
                {
                    return kvp.Key;
                }
            }

            return null;
        }

        public static IEnumerable<DataPoint> GetCachedDataPoints(IHasId id)
        {
            DirectoryInfo cacheDir = id.DataPointDirectory();

            string DataPointFile = $"{cacheDir.FullName}.json";

            foreach (string line in File.ReadAllLines(DataPointFile))
            {
                yield return DataPoint.FromString(line);
            }
        }

        public OrderResponse Buy(Account account, IHasId toPurchase, decimal usdAmount, decimal? price = null)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (toPurchase is null)
            {
                throw new ArgumentNullException(nameof(toPurchase));
            }
            //TODO support partial

            decimal? purchasePrice = price;
            decimal quantity = 0;

            if (purchasePrice is null)
            {
                Quote quote = DownloadJson<Quote>(QuoteUrl(toPurchase));

                if (usdAmount == decimal.MaxValue)
                {
                    decimal avail = GetAccountInformation().CryptoBuyingPower.Amount * 0.985m;

                    quantity = (int)(avail / quote.AskPrice);
                }

                purchasePrice = quote.AskPrice;
            }
            else
            {
                quantity = (int)(usdAmount / price);
            }

            Order order = new()
            {
                AccountId = account.Id,
                CurrencyPairId = toPurchase.Id,
                Price = purchasePrice.Value,
                Quantity = quantity,
                Type = price is null ? "market" : "limit"
            };

            return UploadJson<OrderResponse>(OrderUrl, order);
        }

        public override T DownloadJson<T>(string url, JsonSerializerSettings downloadSerializerSettings = null)
        {
            T toReturn = default;

            do
            {
                try
                {
                    toReturn = LogWebException(() => base.DownloadJson<T>(url, downloadSerializerSettings));
                    break;
                }
                catch (SessionExpiredException)
                {
                    RefreshSession();
                }
            } while (true);

            TryLog(toReturn);

            return toReturn;
        }

        public FindResponse Find(string query)
        {
            return DownloadJson<FindResponse>(SearchUrl(query));
        }

        public CurrencyPair FindCurrency(string query, CurrencyMatch match = CurrencyMatch.CodeOrName)
        {
            List<CurrencyPair> pairs = Find(query).CurrencyPairs.ToList();

            List<CurrencyPair> results = new();

            if (match.HasFlag(CurrencyMatch.Code))
            {
                results.AddRange(pairs.Where(c => string.Equals(c.AssetCurrency.Code, query, StringComparison.OrdinalIgnoreCase)));
            }

            if (match.HasFlag(CurrencyMatch.Name))
            {
                results.AddRange(pairs.Where(c => string.Equals(c.AssetCurrency.Name, query, StringComparison.OrdinalIgnoreCase)));
            }

            return results.Distinct().SingleOrDefault();
        }

        public IHasId FindId(string query, CurrencyMatch currencyMatch = CurrencyMatch.CodeOrName)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (!Tickers.TryGetValue(query.ToUpper(System.Globalization.CultureInfo.CurrentCulture), out string Id))
            {
                CurrencyPair pair = FindCurrency(query, currencyMatch);

                Id = pair.Id;

                Tickers.Add(pair.AssetCurrency.Code, Id);
            }

            return new SymbolResponse()
            {
                Id = Guid.Parse(Id),
                Symbol = query.ToUpper(System.Globalization.CultureInfo.CurrentCulture)
            };
        }

        public AccountInformation GetAccountInformation()
        {
            AccountInformation toReturn = DownloadJson<AccountInformation>(UnifiedAccountUrl);

            return toReturn;
        }

        public DataPoint GetDataPoint(IHasId id, DateTime datePointTime, HistoricalInterval interval, bool cacheOnly = false)
        {
            DateTime parsedRequest = new(datePointTime.Year, datePointTime.Month, datePointTime.Day, datePointTime.Hour, datePointTime.Minute, 0);

            FileInfo cachedFile = id.DataPointFile(parsedRequest, interval);

            if (cachedFile.Exists)
            {
                return JsonConvert.DeserializeObject<DataPoint>(File.ReadAllText(cachedFile.FullName));
            }

            if (cacheOnly)
            {
                return null;
            }

            if (parsedRequest.Minute != 0)
            {
                return null;
            }

            if ((DateTime.Now - parsedRequest).TotalDays < 1)
            {
                _ = RequestHistorical(id, HistoricalSpan._Day, HistoricalInterval._5Minute);
                return GetDataPoint(id, parsedRequest, interval, true);
            }

            if ((DateTime.Now - parsedRequest).TotalDays < 7)
            {
                _ = RequestHistorical(id, HistoricalSpan._Week, HistoricalInterval._5Minute);
                return GetDataPoint(id, parsedRequest, interval, true);
            }

            int WithinMonths = 0;

            DateTime checkDate = DateTime.Now;

            while (checkDate > parsedRequest)
            {
                checkDate = checkDate.AddMonths(-1);
                WithinMonths++;
            }

            if (WithinMonths < 1)
            {
                _ = RequestHistorical(id, HistoricalSpan._Month, HistoricalInterval._Hour);
                return GetDataPoint(id, parsedRequest, interval, true);
            }

            if (WithinMonths < 3)
            {
                _ = RequestHistorical(id, HistoricalSpan._3Month, HistoricalInterval._Hour);
                return GetDataPoint(id, parsedRequest, interval, true);
            }

            if (WithinMonths < 12)
            {
                _ = RequestHistorical(id, HistoricalSpan._Year, HistoricalInterval._Hour);
                return GetDataPoint(id, parsedRequest, interval, true);
            }

            if (WithinMonths < 60)
            {
                _ = RequestHistorical(id, HistoricalSpan._5Year, HistoricalInterval._Hour);
                return GetDataPoint(id, parsedRequest, interval, true);
            }

            return null;
        }

        public Holding GetHolding(IHasId hasId)
        {
            List<Holding> holdings = GetHoldings().ToList();

            return holdings.FirstOrDefault(r => string.Equals(r.Currency.Code, FindSymbol(hasId), StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Holding> GetHoldings()
        {
            return DownloadJson<HoldingsResponse>(HoldingUrl).Results;
        }

        public Quote GetQuote(IHasId id)
        {
            Quote q = DownloadJson<Quote>(QuoteUrl(id));

            DataPoint dp = new()
            {
                BeginsAt = DateTime.Now,
                ClosePrice = q.BidPrice,
                HighPrice = q.HighPrice,
                LowPrice = q.LowPrice,
                Interval = HistoricalInterval._1Second,
                Session = "reg",
                Volume = 0,
                OpenPrice = q.AskPrice,
                Quote = q
            };

            CacheDataPoint(id, dp);

            return q;
        }

        public IEnumerable<Quote> GetQuotes(QuoteRequest request)
        {
            foreach (Quote quote in UploadJson<QuotesResponse>(QuotesUrl, request).Results)
            {
                _ = Tickers.TryAdd(quote.Symbol, $"{quote.Id}");

                yield return quote;
            }
        }

        public IEnumerable<Quote> GetQuotes(IEnumerable<IHasId> Ids)
        {
            return GetQuotes(new QuoteRequest()
            {
                Ids = Ids.ToList()
            });
        }

        public string HistoricalUrl(IHasId Id)
        {
            return Id is null ? throw new ArgumentNullException(nameof(Id)) : $"{Authority}/marketdata/forex/historicals/{Id.Id}/";
        }

        public string QuoteUrl(IHasId id)
        {
            return id is null ? throw new ArgumentNullException(nameof(id)) : $"{Authority}/marketdata/forex/quotes/{id.Id}/";
        }

        public void RefreshSession()
        {
            Authentication = null;

            Authentication = UploadJson<AuthenticationResponse>(AuthUrl, new AuthenticationRequest()
            {
                ClientId = Settings.ClientId,
                DeviceToken = Settings.DeviceToken,
                Password = Settings.Password,
                Username = Settings.Username
            });

            File.WriteAllText(CachedAuthPath, JsonConvert.SerializeObject(Authentication, Formatting.Indented));
        }

        public HistoricalResponse RequestHistorical(IHasId id, HistoricalSpan span, HistoricalInterval interval, HistoricalBounds bounds = HistoricalBounds._24_7)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (span > HistoricalSpan._Week && interval <= HistoricalInterval._5Minute)
            {
                throw new InvalidRequestException("5 minute timespan is only available for week and shorter");
            }

            if (span > HistoricalSpan._Hour && interval <= HistoricalInterval._15Second)
            {
                throw new InvalidRequestException("15 second timespan is only available for hour and shorter");
            }

            HistoricalRequest request = new()
            {
                Id = $"{id.Id}",
                Bounds = bounds,
                Interval = interval,
                Span = span
            };

            HistoricalResponse response = DownloadJson<HistoricalResponse>($"{HistoricalUrl(id)}?{request}");

            foreach (DataPoint dp in response.DataPoints)
            {
                if (dp.BeginsAt != null)
                {
                    dp.Interval = interval;
                }

                CacheDataPoint(response.Id, dp);
            }

            return response;
        }

        public static string SearchUrl(string query)
        {
            return $"https://bonfire.robinhood.com/deprecated_search/?query={query}&user_origin=US";
        }

        /// <summary>
        /// Buys the specified coin with the specified account
        /// </summary>
        /// <param name="account"></param>
        /// <param name="toSell"></param>
        /// <param name="unitsToSell"></param>
        /// <param name="price">if null, uses market. Otherwise, limit</param>
        /// <returns></returns>
        public OrderResponse Sell(Account account, IHasId toSell, decimal unitsToSell, decimal? price = null)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (toSell is null)
            {
                throw new ArgumentNullException(nameof(toSell));
            }

            decimal? sellPrice = price;

            if (unitsToSell == decimal.MaxValue)
            {
                unitsToSell = GetHolding(toSell).QuantityAvailable;
            }

            if (sellPrice is null)
            {
                Quote quote = DownloadJson<Quote>(QuoteUrl(toSell));

                sellPrice = quote.AskPrice;
            }

            Order order = new()
            {
                AccountId = account.Id,
                CurrencyPairId = toSell.Id,
                Price = sellPrice.Value,
                Quantity = unitsToSell,
                Side = "sell",
                Type = price is null ? "market" : "limit"
            };

            return UploadJson<OrderResponse>(OrderUrl, order);
        }

        public override string UploadJson(string url, string toUpload)
        {
            TryLog(toUpload);

            string toReturn = LogWebException(() => base.UploadJson(url, toUpload));

            TryLog(toReturn);

            return toReturn;
        }

        public override T UploadJson<T>(string url, object toUpload, JsonSerializerSettings downloadSerializerSettings = null, JsonSerializerSettings uploadSerializerSettings = null)
        {
            TryLog(toUpload);
            T toReturn = default;

            do
            {
                try
                {
                    toReturn = LogWebException(() => base.UploadJson<T>(url, toUpload, downloadSerializerSettings, uploadSerializerSettings));
                    break;
                }
                catch (SessionExpiredException)
                {
                    RefreshSession();
                }
            } while (true);
            TryLog(toReturn);

            return toReturn;
        }

        protected override void Dispose(bool disposing)
        {
            LogWriter?.Dispose();

            base.Dispose(disposing);
        }

        protected override void PreRequest(Uri url)
        {
            Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36";

            if (Authentication != null)
            {
                Headers[AUTHORIZATION_HEADER] = $"Bearer {Authentication.AccessToken}";
            }
            else
            {
                if (Headers.AllKeys.Contains(AUTHORIZATION_HEADER))
                {
                    Headers.Remove(AUTHORIZATION_HEADER);
                }
            }
        }

        private T LogWebException<T>(Func<T> toExecute)
        {
            try
            {
                return toExecute.Invoke();
            }
            catch (WebException wex) when (wex.Response != null)
            {
                using (StreamReader r = new(wex.Response.GetResponseStream()))
                {
                    string responseContent = r.ReadToEnd();

                    if (responseContent.Contains("Invalid JWT. Signature has expired"))
                    {
                        throw new SessionExpiredException();
                    }

                    LogWriter.WriteLine(responseContent);
                }

                throw;
            }
        }

        private void TryLog(object o)
        {
            try
            {
                LogWriter.WriteLine(o);
            }
            catch (Exception ex)
            {
                LogWriter.WriteLine("An exception occured while logging.");
                LogWriter.WriteLine(ex.Message);
                LogWriter.WriteLine(ex.StackTrace);
            }
        }
    }
}