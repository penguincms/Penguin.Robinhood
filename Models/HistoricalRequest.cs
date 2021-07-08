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
        public string StrBounds => this.Bounds.ToString().Trim('_').ToLower();

        [HttpQueryProperty("interval")]
        public string StrInterval => this.Interval.ToString().Trim('_').ToLower();

        [HttpQueryProperty("span")]
        public string StrSpan => this.Span.ToString().Trim('_').ToLower();
    }

    public enum HistoricalBounds
    {
        _24_7
    }

    public enum HistoricalInterval
    {
        _Day = 86400,
        _Week = 604800,
        _Hour = 3600,
        _5Minute = 300,
        _15Second = 15,
        _1Second = 1
    }

    public enum HistoricalSpan
    {
        _Day = 24,
        _Year = 8760,
        _5Year = 43800,
        _3Month = 2160,
        _Month = 720,
        _Week = 168,
        _Hour = 1
    }
}