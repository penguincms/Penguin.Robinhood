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
        public long BeginsTicks => this.BeginsAt?.Ticks ?? 0;

        [JsonProperty("close_price")]
        public decimal? ClosePrice { get; set; }

        public DateTime? EndsAt => this.BeginsAt?.AddMinutes((int)this.Interval);

        [JsonIgnore]
        public long EndsTicks => this.EndsAt?.Ticks ?? 0;

        [JsonProperty("high_price")]
        public decimal? HighPrice { get; set; }

        [JsonProperty("interpolated")]
        public bool Interpolated { get; set; }

        public HistoricalInterval Interval { get; set; }

        [JsonProperty("low_price")]
        public decimal? LowPrice { get; set; }

        [JsonProperty("open_price")]
        public virtual decimal? OpenPrice { get; set; }

        decimal IPricePoint.Price => this.OpenPrice.Value;
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

            DataPoint dp = new DataPoint();

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
            return $"{this.BeginsAt?.Ticks}\t{(int)this.Interval}\t{this.OpenPrice}\t{this.ClosePrice}\t{this.HighPrice}\t{this.LowPrice}\t{this.Volume}\t{this.Session}\t{(this.Interpolated ? 0 : 1)}\t{this.Quote?.AskPrice}\t{this.Quote?.BidPrice}\t{this.Quote?.MarkPrice}\t{this.Quote?.HighPrice}\t{this.Quote?.LowPrice}\t{this.Quote?.OpenPrice}\t{this.Quote?.Symbol}\t{this.Quote?.Id}\t{this.Quote?.Volume}";
        }
    }
}