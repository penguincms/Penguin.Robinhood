namespace Penguin.Robinhood.Models
{
    public enum HistoricalInterval
    {
        None,
        _Day = 86400,
        _Week = 604800,
        _Hour = 3600,
        _5Minute = 300,
        _15Second = 15,
        _1Second = 1
    }
}