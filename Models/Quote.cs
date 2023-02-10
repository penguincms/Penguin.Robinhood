using Newtonsoft.Json;
using Penguin.Robinhood.Interfaces;
using System;

namespace Penguin.Robinhood.Models
{
    public class Quote : LoggedObject, IPricePoint
    {
        [JsonProperty("ask_price")]
        public decimal AskPrice { get; set; }

        [JsonProperty("bid_price")]
        public decimal BidPrice { get; set; }

        [JsonProperty("high_price")]
        public decimal HighPrice { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("low_price")]
        public decimal LowPrice { get; set; }

        [JsonProperty("mark_price")]
        public decimal MarkPrice { get; set; }

        [JsonProperty("open_price")]
        public decimal OpenPrice { get; set; }

        decimal IPricePoint.Price => AskPrice;

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("volume")]
        public decimal Volume { get; set; }
    }
}