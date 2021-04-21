using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Penguin.Robinhood.Models
{
    public class Account : LoggedObject
    {
        [JsonProperty("apex_account_number")]
        public string ApexAccountNumber { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("rhs_account_number")]
        public string RhsAccountNumber { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status_reason_code")]
        public string StatusReasonCode { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    public class AccountsResponse
    {
        [JsonProperty("results")]
        public List<Account> Results { get; set; }
    }
}