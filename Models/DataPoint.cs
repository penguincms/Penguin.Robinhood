using Newtonsoft.Json;
using Penguin.Robinhood.Interfaces;
using System;

namespace Penguin.Robinhood.Models
{
    public class DataPoint : LoggedObject, IPricePoint
    {
        [JsonProperty("begins_at")]
        public DateTime? BeginsAt { get; set; }

        [JsonIgnore]
        public long BeginsTicks => BeginsAt?.Ticks ?? 0;

        [JsonProperty("close_price")]
        public decimal? ClosePrice { get; set; }

        public DateTime? EndsAt => BeginsAt?.AddMinutes((int)Interval);

        [JsonIgnore]
        public long EndsTicks => EndsAt?.Ticks ?? 0;

        [JsonProperty("high_price")]
        public decimal? HighPrice { get; set; }

        [JsonProperty("interpolated")]
        public bool Interpolated { get; set; }

        public HistoricalInterval Interval { get; set; }

        [JsonProperty("low_price")]
        public decimal? LowPrice { get; set; }

        [JsonProperty("open_price")]
        public virtual decimal? OpenPrice { get; set; }

        decimal IPricePoint.Price => OpenPrice.Value;

        public Quote Quote { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        public static DataPoint FromString(string line)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            DataPoint dp = new();

            string[] a = line.Split('\t');

            dp.BeginsAt = new DateTime(long.Parse(a[0]));
            dp.Interval = (HistoricalInterval)int.Parse(a[1]);
            dp.OpenPrice = decimal.Parse(a[2]);
            dp.ClosePrice = decimal.Parse(a[3]);
            dp.HighPrice = decimal.Parse(a[4]);
            dp.LowPrice = decimal.Parse(a[5]);
            dp.Volume = int.Parse(a[6]);
            dp.Session = a[7];
            dp.Interpolated = a[8] == "1";

            if (!string.IsNullOrWhiteSpace(a[9]))
            {
                dp.Quote = new Quote
                {
                    AskPrice = decimal.Parse(a[9]),
                    BidPrice = decimal.Parse(a[10]),
                    MarkPrice = decimal.Parse(a[11]),
                    HighPrice = decimal.Parse(a[12]),
                    LowPrice = decimal.Parse(a[13]),
                    OpenPrice = decimal.Parse(a[14]),
                    Symbol = a[15],
                    Id = Guid.Parse(a[16]),
                    Volume = decimal.Parse(a[17])
                };
            }

            return dp;
        }

        public override string ToString()
        {
            return $"{BeginsAt?.Ticks}\t{(int)Interval}\t{OpenPrice}\t{ClosePrice}\t{HighPrice}\t{LowPrice}\t{Volume}\t{Session}\t{(Interpolated ? 0 : 1)}\t{Quote?.AskPrice}\t{Quote?.BidPrice}\t{Quote?.MarkPrice}\t{Quote?.HighPrice}\t{Quote?.LowPrice}\t{Quote?.OpenPrice}\t{Quote?.Symbol}\t{Quote?.Id}\t{Quote?.Volume}";
        }
    }
}