using Newtonsoft.Json;

namespace Penguin.Robinhood.Models
{
    public class AuthenticationRequest : LoggedObject
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("device_token")]
        public string DeviceToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; } = 86400;

        [JsonProperty("grant_type")]
        public string GrantType { get; set; } = "password";

        [JsonProperty("scope")]
        public string Scope { get; set; } = "internal";

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}