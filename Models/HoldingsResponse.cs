using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Penguin.Robinhood.Models
{
    public class CostBas : LoggedObject
    {
        [JsonProperty("currency_id")]
        public Guid CurrencyId { get; set; }

        [JsonProperty("direct_cost_basis")]
        public string DirectCostBasis { get; set; }

        [JsonProperty("direct_quantity")]
        public string DirectQuantity { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("intraday_cost_basis")]
        public string IntradayCostBasis { get; set; }

        [JsonProperty("intraday_quantity")]
        public string IntradayQuantity { get; set; }

        [JsonProperty("marked_cost_basis")]
        public string MarkedCostBasis { get; set; }

        [JsonProperty("marked_quantity")]
        public string MarkedQuantity { get; set; }
    }

    public class Currency : LoggedObject
    {
        [JsonProperty("brand_color")]
        public string BrandColor { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("increment")]
        public string Increment { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Holding : LoggedObject
    {
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("cost_bases")]
        public List<CostBas> CostBases { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("currency")]
        public Currency Currency { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("quantity_available")]
        public decimal QuantityAvailable { get; set; }

        [JsonProperty("quantity_held_for_buy")]
        public decimal QuantityHeldForBuy { get; set; }

        [JsonProperty("quantity_held_for_sell")]
        public decimal QuantityHeldForSell { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class HoldingsResponse
    {
        [JsonProperty("results")]
        public List<Holding> Results { get; set; }
    }
}