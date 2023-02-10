using Penguin.Web.Http;
using Penguin.Web.Http.Attributes;

namespace Penguin.Robinhood.Models
{
    public class HistoricalRequest : HttpQuery
    {
        [IgnoreQueryProperty]
        public HistoricalBounds Bounds { get; set; } = HistoricalBounds._24_7;

        [IgnoreQueryProperty]
        public string Id { get; set; }

        [IgnoreQueryProperty]
        public HistoricalInterval Interval { get; set; }

        [IgnoreQueryProperty]
        public HistoricalSpan Span { get; set; }

        [HttpQueryProperty("bounds")]
        public string StrBounds => Bounds.ToString().Trim('_').ToLower(System.Globalization.CultureInfo.CurrentCulture);

        [HttpQueryProperty("interval")]
        public string StrInterval => Interval.ToString().Trim('_').ToLower(System.Globalization.CultureInfo.CurrentCulture);

        [HttpQueryProperty("span")]
        public string StrSpan => Span.ToString().Trim('_').ToLower(System.Globalization.CultureInfo.CurrentCulture);
    }
}