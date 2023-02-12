using Penguin.Web.Http;
using Loxifi.Attributes;
using Loxifi;

namespace Penguin.Robinhood.Models
{
    public class HistoricalRequest : HttpQuery
    {
        [HttpQueryPropertyIgnore]
        public HistoricalBounds Bounds { get; set; } = HistoricalBounds._24_7;

        [HttpQueryPropertyIgnore]
        public string Id { get; set; }

        [HttpQueryPropertyIgnore]
        public HistoricalInterval Interval { get; set; }

        [HttpQueryPropertyIgnore]
        public HistoricalSpan Span { get; set; }

        [HttpQueryProperty("bounds")]
        public string StrBounds => Bounds.ToString().Trim('_').ToLower(System.Globalization.CultureInfo.CurrentCulture);

        [HttpQueryProperty("interval")]
        public string StrInterval => Interval.ToString().Trim('_').ToLower(System.Globalization.CultureInfo.CurrentCulture);

        [HttpQueryProperty("span")]
        public string StrSpan => Span.ToString().Trim('_').ToLower(System.Globalization.CultureInfo.CurrentCulture);
    }
}