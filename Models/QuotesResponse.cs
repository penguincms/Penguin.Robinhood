using Newtonsoft.Json;
using System.Collections.Generic;

namespace Penguin.Robinhood.Models
{
    public class QuotesResponse : LoggedObject
    {
        [JsonProperty("results")]
        public List<Quote> Results { get; set; }
    }
}