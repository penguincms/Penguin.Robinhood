using Newtonsoft.Json;
using System;

namespace Penguin.Robinhood.Models
{
    public class AuthenticationResponse : LoggedObject
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public bool IsExpired => DateTime.Now > this.CreatedAt.AddMinutes(this.ExpiresIn - 10);

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("accept_challenge_types")]
        public AcceptChallengeTypes AcceptChallengeTypes { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("mfa_code")]
        public object MfaCode { get; set; }

        [JsonProperty("backup_code")]
        public object BackupCode { get; set; }
    }

    public class AcceptChallengeTypes
    {
        [JsonProperty("sms")]
        public string Sms { get; set; }
    }
}