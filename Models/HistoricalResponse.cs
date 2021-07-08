using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Penguin.Robinhood.Models
{
    public class HistoricalResponse : LoggedObject
    {
        [JsonProperty("bounds")]
        public string Bounds { get; set; }

        [JsonProperty("data_points")]
        public List<DataPoint> DataPoints { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("interval")]
        public string Interval { get; set; }

        [JsonProperty("open_price")]
        public decimal? OpenPrice { get; set; }

        [JsonProperty("open_time")]
        public DateTime? OpenTime { get; set; }

        [JsonProperty("previous_close_price")]
        public decimal? PreviousClosePrice { get; set; }

        [JsonProperty("previous_close_time")]
        public DateTime? PreviousCloseTime { get; set; }

        [JsonProperty("span")]
        public string Span { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}