using Newtonsoft.Json;
using System;

namespace Penguin.Robinhood.Models
{
    public class Origin
    {
        [JsonProperty("locality")]
        public string Locality { get; set; }
    }

    public class UserResponse : LoggedObject
    {
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("id_info")]
        public string IdInfo { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("origin")]
        public Origin Origin { get; set; }

        [JsonProperty("profile_name")]
        public string ProfileName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}