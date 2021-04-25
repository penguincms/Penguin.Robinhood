using Newtonsoft.Json;
using Penguin.Robinhood.Interfaces;
using System;

namespace Penguin.Robinhood.Models
{
    public class DataPoint : LoggedObject, IPricePoint
    {
        public Quote Quote { get; set; }

        [JsonIgnore]
        public long BeginsTicks => this.BeginsAt?.Ticks ?? 0;

        [JsonIgnore]
        public long EndsTicks => this.EndsAt?.Ticks ?? 0;

        [JsonProperty("begins_at")]
        public DateTime? BeginsAt { get; set; }

        [JsonProperty("open_price")]
        public virtual decimal? OpenPrice { get; set; }

        [JsonProperty("close_price")]
        public decimal? ClosePrice { get; set; }

        [JsonProperty("high_price")]
        public decimal? HighPrice { get; set; }

        [JsonProperty("low_price")]
        public decimal? LowPrice { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("interpolated")]
        public bool Interpolated { get; set; }

        public DateTime? EndsAt => this.BeginsAt?.AddMinutes((int)this.Interval);

        public HistoricalInterval Interval { get; set; }

        decimal IPricePoint.Price => this.OpenPrice.Value;

        public override string ToString()
        {
            return $"{BeginsAt?.Ticks}\t{(int)Interval}\t{OpenPrice}\t{ClosePrice}\t{HighPrice}\t{LowPrice}\t{Volume}\t{Session}\t{(Interpolated ? 0 : 1)}\t{Quote?.AskPrice}\t{Quote?.BidPrice}\t{Quote?.MarkPrice}\t{Quote?.HighPrice}\t{Quote?.LowPrice}\t{Quote?.OpenPrice}\t{Quote?.Symbol}\t{Quote?.Id}\t{Quote?.Volume}";
        }

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
                dp.Quote = new Quote();
                dp.Quote.AskPrice = decimal.Parse(a[9]);
                dp.Quote.BidPrice = decimal.Parse(a[10]);
                dp.Quote.MarkPrice = decimal.Parse(a[11]);
                dp.Quote.HighPrice = decimal.Parse(a[12]);
                dp.Quote.LowPrice = decimal.Parse(a[13]);
                dp.Quote.OpenPrice = decimal.Parse(a[14]);
                dp.Quote.Symbol = a[15];
                dp.Quote.Id = Guid.Parse(a[16]);
                dp.Quote.Volume = decimal.Parse(a[17]);
            }

            return dp;
        }
    }
}