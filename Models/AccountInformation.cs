using Newtonsoft.Json;
using System;

namespace Penguin.Robinhood.Models
{
    public class AccountInformation
    {
        [JsonProperty("account_buying_power")]
        public CurrencyAmount AccountBuyingPower { get; set; }

        [JsonProperty("cash_available_from_instant_deposits")]
        public CurrencyAmount CashAvailableFromInstantDeposits { get; set; }

        [JsonProperty("cash_held_for_currency_orders")]
        public CurrencyAmount CashHeldForCurrencyOrders { get; set; }

        [JsonProperty("cash_held_for_dividends")]
        public CurrencyAmount CashHeldForDividends { get; set; }

        [JsonProperty("cash_held_for_equity_orders")]
        public CurrencyAmount CashHeldForEquityOrders { get; set; }

        [JsonProperty("cash_held_for_options_collateral")]
        public CurrencyAmount CashHeldForOptionsCollateral { get; set; }

        [JsonProperty("cash_held_for_orders")]
        public CurrencyAmount CashHeldForOrders { get; set; }

        [JsonProperty("crypto")]
        public Crypto Crypto { get; set; }

        [JsonProperty("crypto_buying_power")]
        public CurrencyAmount CryptoBuyingPower { get; set; }

        [JsonProperty("equities")]
        public Equities Equities { get; set; }

        [JsonProperty("extended_hours_portfolio_equity")]
        public CurrencyAmount ExtendedHoursPortfolioEquity { get; set; }

        [JsonProperty("instant_allocated")]
        public CurrencyAmount InstantAllocated { get; set; }

        [JsonProperty("levered_amount")]
        public CurrencyAmount LeveredAmount { get; set; }

        [JsonProperty("near_margin_call")]
        public bool NearMarginCall { get; set; }

        [JsonProperty("options_buying_power")]
        public CurrencyAmount OptionsBuyingPower { get; set; }

        [JsonProperty("portfolio_equity")]
        public CurrencyAmount PortfolioEquity { get; set; }

        [JsonProperty("portfolio_previous_close")]
        public CurrencyAmount PortfolioPreviousClose { get; set; }

        [JsonProperty("previous_close")]
        public CurrencyAmount PreviousClose { get; set; }

        [JsonProperty("regular_hours_portfolio_equity")]
        public CurrencyAmount RegularHoursPortfolioEquity { get; set; }

        [JsonProperty("total_equity")]
        public CurrencyAmount TotalEquity { get; set; }

        [JsonProperty("total_extended_hours_equity")]
        public CurrencyAmount TotalExtendedHoursEquity { get; set; }

        [JsonProperty("total_extended_hours_market_value")]
        public CurrencyAmount TotalExtendedHoursMarketValue { get; set; }

        [JsonProperty("total_market_value")]
        public CurrencyAmount TotalMarketValue { get; set; }

        [JsonProperty("total_regular_hours_equity")]
        public CurrencyAmount TotalRegularHoursEquity { get; set; }

        [JsonProperty("total_regular_hours_market_value")]
        public CurrencyAmount TotalRegularHoursMarketValue { get; set; }

        [JsonProperty("uninvested_cash")]
        public CurrencyAmount UninvestedCash { get; set; }

        [JsonProperty("withdrawable_cash")]
        public CurrencyAmount WithdrawableCash { get; set; }
    }

    public class Crypto
    {
        [JsonProperty("equity")]
        public CurrencyAmount Equity { get; set; }

        [JsonProperty("market_value")]
        public CurrencyAmount MarketValue { get; set; }

        [JsonProperty("opened_at")]
        public DateTime OpenedAt { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CurrencyAmount
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("currency_id")]
        public Guid CurrencyId { get; set; }
    }

    public class Equities
    {
        [JsonProperty("active_subscription_id")]
        public object ActiveSubscriptionId { get; set; }

        [JsonProperty("apex_account_number")]
        public object ApexAccountNumber { get; set; }

        [JsonProperty("available_margin")]
        public object AvailableMargin { get; set; }

        [JsonProperty("equity")]
        public CurrencyAmount Equity { get; set; }

        [JsonProperty("margin_maintenance")]
        public CurrencyAmount MarginMaintenance { get; set; }

        [JsonProperty("market_value")]
        public CurrencyAmount MarketValue { get; set; }

        [JsonProperty("opened_at")]
        public DateTime OpenedAt { get; set; }

        [JsonProperty("rhs_account_number")]
        public string RhsAccountNumber { get; set; }

        [JsonProperty("total_margin")]
        public CurrencyAmount TotalMargin { get; set; }
    }
}