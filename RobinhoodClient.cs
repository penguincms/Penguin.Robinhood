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
        protected override void Dispose(bool disposing)
        {
            this.LogWriter?.Dispose();

            base.Dispose(disposing);
        }

        private string LogDirectory
        {
            get
            {
                return Path.Combine(AppData, "Logs", $"{Process.GetCurrentProcess().ProcessName}");
            }
        }

        public readonly LogWriter LogWriter;

        public static string AppData
        {
            get
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Robinhood");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return directory;
            }
        }

        public static readonly DictionaryFile Tickers = new DictionaryFile(Path.Combine(AppData, "Tickers.Cache"));

        public string Authority = "https://api.robinhood.com";
        public string AuthUrl => $"{this.Authority}/oauth2/token/";
        public string UserUrl => $"{this.Authority}/user/";
        public string AccountsUrl => $"https://nummus.robinhood.com/accounts/";
        public string QuotesUrl => $"{this.Authority}/marketdata/forex/quotes/";
        public string HoldingUrl => $"https://nummus.robinhood.com/holdings/";

        public string HistoricalUrl(IHasId Id)
        {
            return $"{this.Authority}/marketdata/forex/historicals/{Id.Id}/";
        }

        public string OrderUrl => $"https://nummus.robinhood.com/orders/";

        public string QuoteUrl(IHasId id)
        {
            return $"{this.Authority}/marketdata/forex/quotes/{id.Id}/";
        }

        public IEnumerable<Holding> GetHoldings()
        {
            return this.DownloadJson<HoldingsResponse>(this.HoldingUrl).Results;
        }

        public Holding GetHolding(IHasId hasId)
        {
            List<Holding> holdings = this.GetHoldings().ToList();
            
            return holdings.FirstOrDefault(r => string.Equals(r.Currency.Code, FindSymbol(hasId), StringComparison.OrdinalIgnoreCase));
        }

        public AccountInformation GetAccountInformation()
        {
            AccountInformation toReturn = this.DownloadJson<AccountInformation>(this.UnifiedAccountUrl);

            return toReturn;
        }

        public OrderResponse Buy(Account account, IHasId toPurchase, decimal quantity, decimal adjust = 0)
        {
            //TODO support partial

            Quote quote = this.DownloadJson<Quote>(this.QuoteUrl(toPurchase));

            if (quantity == decimal.MaxValue)
            {
                decimal avail = this.GetAccountInformation().CryptoBuyingPower.Amount * 0.985m;

                quantity = (int)(avail / quote.AskPrice);
            }

            Order order = new Order
            {
                AccountId = account.Id,
                CurrencyPairId = toPurchase.Id,
                Price = quote.AskPrice + adjust,
                Quantity = quantity
            };

            return this.UploadJson<OrderResponse>(this.OrderUrl, order);
        }

        public OrderResponse Sell(Account account, IHasId toSell, decimal quantity, decimal adjust = 0)
        {
            Quote quote = this.DownloadJson<Quote>(this.QuoteUrl(toSell));

            if (quantity == decimal.MaxValue)
            {
                quantity = this.GetHolding(toSell).QuantityAvailable;
            }

            Order order = new Order
            {
                AccountId = account.Id,
                CurrencyPairId = toSell.Id,
                Price = quote.AskPrice + adjust,
                Quantity = quantity,
                Side = "sell"
            };

            return this.UploadJson<OrderResponse>(this.OrderUrl, order);
        }

        public string SearchUrl(string query)
        {
            return $"https://bonfire.robinhood.com/deprecated_search/?query={query}&user_origin=US";
        }

        public static IEnumerable<DataPoint> GetCachedDataPoints(IHasId id)
        {
            DirectoryInfo cacheDir = id.DataPointDirectory();

            foreach (FileInfo f in cacheDir.EnumerateFiles())
            {
                yield return JsonConvert.DeserializeObject<DataPoint>(File.ReadAllText(f.FullName));
            }
        }

        public UserResponse User { get; private set; }

        private readonly AuthenticationResponse Authentication;

        public IReadOnlyList<Account> Accounts { get; private set; }

        public string UnifiedAccountUrl => $"https://phoenix.robinhood.com/accounts/unified";

        protected override void PreRequest(Uri url)
        {
            this.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36";

            if (this.Authentication != null)
            {
                this.Headers["Authorization"] = $"Bearer {this.Authentication.AccessToken}";
            }
        }

        public RobinhoodClient(IAuthenticationSettings settings)
        {
            this.LogWriter = new LogWriter(this.LogDirectory)
            {
                ObjectSerializationOverride = JsonConvert.SerializeObject
            };

            string cachedAuthPath = Path.Combine(AppData, "Token.json");

            AuthenticationResponse existingAuth = null;

            if (File.Exists(cachedAuthPath))
            {
                existingAuth = JsonConvert.DeserializeObject<AuthenticationResponse>(File.ReadAllText(cachedAuthPath));

                if (existingAuth.IsExpired)
                {
                    existingAuth = null;
                }
            }

            if (existingAuth is null)
            {
                this.Authentication = this.UploadJson<AuthenticationResponse>(this.AuthUrl, new AuthenticationRequest()
                {
                    ClientId = settings.ClientId,
                    DeviceToken = settings.DeviceToken,
                    Password = settings.Password,
                    Username = settings.Username
                });

                File.WriteAllText(cachedAuthPath, JsonConvert.SerializeObject(this.Authentication, Formatting.Indented));
            }
            else
            {
                this.Authentication = existingAuth;
            }

            this.User = this.DownloadJson<UserResponse>(this.UserUrl);

            this.Accounts = this.DownloadJson<AccountsResponse>(this.AccountsUrl).Results;
        }

        public DataPoint GetDataPoint(IHasId id, DateTime datePointTime, HistoricalInterval interval, bool cacheOnly = false)
        {
            DateTime parsedRequest = new DateTime(datePointTime.Year, datePointTime.Month, datePointTime.Day, datePointTime.Hour, datePointTime.Minute, 0);

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
                this.RequestHistorical(id, HistoricalSpan._Day, HistoricalInterval._5Minute);
                return this.GetDataPoint(id, parsedRequest, interval, true);
            }

            if ((DateTime.Now - parsedRequest).TotalDays < 7)
            {
                this.RequestHistorical(id, HistoricalSpan._Week, HistoricalInterval._5Minute);
                return this.GetDataPoint(id, parsedRequest, interval, true);
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
                this.RequestHistorical(id, HistoricalSpan._Month, HistoricalInterval._Hour);
                return this.GetDataPoint(id, parsedRequest, interval, true);
            }

            if (WithinMonths < 3)
            {
                this.RequestHistorical(id, HistoricalSpan._3Month, HistoricalInterval._Hour);
                return this.GetDataPoint(id, parsedRequest, interval, true);
            }

            if (WithinMonths < 12)
            {
                this.RequestHistorical(id, HistoricalSpan._Year, HistoricalInterval._Hour);
                return this.GetDataPoint(id, parsedRequest, interval, true);
            }

            if (WithinMonths < 60)
            {
                this.RequestHistorical(id, HistoricalSpan._5Year, HistoricalInterval._Hour);
                return this.GetDataPoint(id, parsedRequest, interval, true);
            }

            return null;
        }

        private void TryLog(object o)
        {
            try
            {
                this.LogWriter.WriteLine(o);
            }
            catch (Exception ex)
            {
                this.LogWriter.WriteLine("An exception occured while logging.");
                this.LogWriter.WriteLine(ex.Message);
                this.LogWriter.WriteLine(ex.StackTrace);
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
                using (StreamReader r = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string responseContent = r.ReadToEnd();

                    this.LogWriter.WriteLine(responseContent);
                }

                throw;
            }
        }

        public override string UploadJson(string url, string toUpload)
        {
            this.TryLog(toUpload);

            string toReturn = this.LogWebException(() => base.UploadJson(url, toUpload));

            this.TryLog(toReturn);

            return toReturn;
        }

        public override T UploadJson<T>(string url, object toUpload, JsonSerializerSettings downloadSerializerSettings = null, JsonSerializerSettings uploadSerializerSettings = null)
        {
            this.TryLog(toUpload);

            T toReturn = this.LogWebException(() => base.UploadJson<T>(url, toUpload, downloadSerializerSettings, uploadSerializerSettings));

            this.TryLog(toReturn);

            return toReturn;
        }

        public override T DownloadJson<T>(string url, JsonSerializerSettings downloadSerializerSettings = null)
        {
            T toReturn = this.LogWebException(() => base.DownloadJson<T>(url, downloadSerializerSettings));

            this.TryLog(toReturn);

            return toReturn;
        }

        public HistoricalResponse RequestHistorical(IHasId id, HistoricalSpan span, HistoricalInterval interval, HistoricalBounds bounds = HistoricalBounds._24_7)
        {
            if (span > HistoricalSpan._Week && interval <= HistoricalInterval._5Minute)
            {
                throw new InvalidRequestException("5 minute timespan is only available for week and shorter");
            }

            if (span > HistoricalSpan._Hour && interval <= HistoricalInterval._15Second)
            {
                throw new InvalidRequestException("15 second timespan is only available for hour and shorter");
            }

            HistoricalRequest request = new HistoricalRequest()
            {
                Id = $"{id.Id}",
                Bounds = bounds,
                Interval = interval,
                Span = span
            };

            HistoricalResponse response = this.DownloadJson<HistoricalResponse>($"{this.HistoricalUrl(id)}?{request}");

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
            FileInfo dataPointFile = id.DataPointFile(dp);

            if (!dataPointFile.Exists)
            {
                File.WriteAllText(dataPointFile.FullName, JsonConvert.SerializeObject(dp, Formatting.Indented));
            }
        }

        public IEnumerable<Quote> GetQuotes(QuoteRequest request)
        {
            foreach (Quote quote in this.UploadJson<QuotesResponse>(this.QuotesUrl, request).Results)
            {
                Tickers.TryAdd(quote.Symbol, $"{quote.Id}");

                yield return quote;
            }
        }
        public Quote GetQuote(IHasId id)
        {
            return this.DownloadJson<Quote>(this.QuoteUrl(id));
        }

        public static string FindSymbol(IHasId id) => FindSymbol(id.Id);

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

        public FindResponse Find(string query)
        {
            return this.DownloadJson<FindResponse>(this.SearchUrl(query));
        }

        public CurrencyPair FindCurrency(string query, CurrencyMatch match = CurrencyMatch.CodeOrName)
        {
            List<CurrencyPair> pairs = this.Find(query).CurrencyPairs.ToList();

            List<CurrencyPair> results = new List<CurrencyPair>();

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
            if (!Tickers.TryGetValue(query.ToUpper(), out string Id))
            {
                CurrencyPair pair = this.FindCurrency(query, currencyMatch);

                Id = pair.Id;

                Tickers.Add(pair.AssetCurrency.Code, Id);
            }

            return new SymbolResponse()
            {
                Id = Guid.Parse(Id),
                Symbol = query.ToUpper()
            };
        }

        public IEnumerable<Quote> GetQuotes(IEnumerable<IHasId> Ids)
        {
            return this.GetQuotes(new QuoteRequest()
            {
                Ids = Ids.ToList()
            });
        }
    }
}