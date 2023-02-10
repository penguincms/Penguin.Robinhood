using Penguin.Robinhood.Interfaces;
using Penguin.Web.Http;
using Penguin.Web.Http.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Robinhood.Models
{
    // https://api.robinhood.com/marketdata/forex/quotes/?ids=3d961844-d360-45fc-989b-f6fca761d511%2C76637d50-c702-4ed1-bcb5-5b0732a81f48%2C2f2b77c4-e426-4271-ae49-18d5cb296d3a%2C383280b1-ff53-43fc-9c84-f01afd0989cd%2C1ef78e1b-049b-4f12-90e5-555dcf2fe204%2C5f1325b6-f63c-4367-9d6f-713e3a0c5d76%2C7837d558-0fe9-4287-8f3e-6de592db127c%2C7b577ce3-489d-4269-9408-796a0d1abb3a%2C7a04fe7a-e3a8-4a07-8c35-d0fec9f35569%2Cb9729798-2aec-4ca9-8637-4d9789d63764%2C35f0496d-6c3a-4cac-9d2f-6702a8c387eb%2Ccc2eb8d1-c42d-4f12-8801-1c4bbe43a274%2C1461976e-a656-481a-af27-dc6f2980e967%2Ca31d3fe3-38e6-4adf-ab4b-e303349f5ee4%2C2de36458-56cf-458d-b76a-6b3f61b2034c%2Cbab5ccb4-6729-416e-ac75-019d650016c9%2C086a8f9f-6c39-43fa-ac9f-57952f4a1ba6
    public class QuoteRequest : HttpQuery
    {
        [IgnoreQueryProperty]
        public List<IHasId> Ids { get; set; } = new List<IHasId>();

        [HttpQueryProperty("ids")]
        public string IdString => string.Join("%2C", Ids.Select(i => i.Id));
    }
}