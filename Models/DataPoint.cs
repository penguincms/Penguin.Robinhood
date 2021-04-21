using Newtonsoft.Json;
using Penguin.Robinhood.Interfaces;
using System;

namespace Penguin.Robinhood.Models
{
    public class DataPoint : LoggedObject, IPricePoint
    {
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
            return $"{this.BeginsAt} - {this.OpenPrice}";
        }
    }
}