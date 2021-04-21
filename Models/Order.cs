using Newtonsoft.Json;
using System;

namespace Penguin.Robinhood.Models
{
    public class Order : LoggedObject
    {
        [JsonProperty("account_id")]
        public Guid AccountId { get; set; }

        [JsonProperty("currency_pair_id")]
        public Guid CurrencyPairId { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("ref_id")]
        public Guid RefId { get; set; } = Guid.NewGuid();

        [JsonProperty("side")]
        public string Side { get; set; } = "buy";

        [JsonProperty("time_in_force")]
        public string TimeInForce { get; set; } = "gtc";

        [JsonProperty("type")]
        public string Type { get; set; } = "market";
    }
}