using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Penguin.Robinhood.Models
{
    public class OrderResponse : LoggedObject
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("average_price")]
        public object AveragePrice { get; set; }

        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("cumulative_quantity")]
        public string CumulativeQuantity { get; set; }

        [JsonProperty("currency_pair_id")]
        public string CurrencyPairId { get; set; }

        [JsonProperty("executions")]
        public List<object> Executions { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("last_transaction_at")]
        public object LastTransactionAt { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("ref_id")]
        public string RefId { get; set; }

        [JsonProperty("rounded_executed_notional")]
        public string RoundedExecutedNotional { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("time_in_force")]
        public string TimeInForce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}