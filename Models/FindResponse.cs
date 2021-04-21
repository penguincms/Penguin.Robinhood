using Newtonsoft.Json;
using System.Collections.Generic;

namespace Penguin.Robinhood.Models
{
    public class FindResponse : LoggedObject
    {
        [JsonProperty("currency_pairs")]
        public List<CurrencyPair> CurrencyPairs { get; set; }
    }

    public class AssetCurrency : LoggedObject
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

    public class QuoteCurrency : LoggedObject
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

    public class CurrencyPair : LoggedObject
    {
        [JsonProperty("asset_currency")]
        public AssetCurrency AssetCurrency { get; set; }

        [JsonProperty("display_only")]
        public bool DisplayOnly { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("max_order_size")]
        public string MaxOrderSize { get; set; }

        [JsonProperty("min_order_price_increment")]
        public string MinOrderPriceIncrement { get; set; }

        [JsonProperty("min_order_quantity_increment")]
        public string MinOrderQuantityIncrement { get; set; }

        [JsonProperty("min_order_size")]
        public string MinOrderSize { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quote_currency")]
        public QuoteCurrency QuoteCurrency { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("tradability")]
        public string Tradability { get; set; }
    }
}