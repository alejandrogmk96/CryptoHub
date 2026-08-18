using System.Globalization;
using CryptoHub.Api.Models;
using CryptoHub.Api.Models.External.BingX;

namespace CryptoHub.Api.Services;

public static class BingXCandleMapper
{
    public static Candle ToCandle(
        BingXCandle bingXCandle,
        TradingPair tradingPair)
    {
        return new Candle
        {
            TradingPair = tradingPair,

            Open = decimal.Parse(
                bingXCandle.Open,
                CultureInfo.InvariantCulture),

            High = decimal.Parse(
                bingXCandle.High,
                CultureInfo.InvariantCulture),

            Low = decimal.Parse(
                bingXCandle.Low,
                CultureInfo.InvariantCulture),

            Close = decimal.Parse(
                bingXCandle.Close,
                CultureInfo.InvariantCulture),

            Volume = decimal.Parse(
                bingXCandle.Volume,
                CultureInfo.InvariantCulture),

            Timestamp =
                DateTimeOffset.FromUnixTimeMilliseconds(
                    bingXCandle.Time)
        };
    }
}