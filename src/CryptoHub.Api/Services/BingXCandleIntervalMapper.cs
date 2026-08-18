using CryptoHub.Api.Models;

namespace CryptoHub.Api.Services;

public static class BingXCandleIntervalMapper
{
    public static string ToBingXInterval(CandleInterval interval)
    {
        return interval switch
        {
            CandleInterval.OneMinute => "1m",
            CandleInterval.FiveMinutes => "5m",
            CandleInterval.FifteenMinutes => "15m",
            CandleInterval.OneHour => "1h",
            CandleInterval.FourHours => "4h",
            CandleInterval.OneDay => "1d",

            _ => throw new ArgumentOutOfRangeException(
                nameof(interval),
                interval,
                "Intervalo de vela no soportado por CryptoHub.")
        };
    }
}