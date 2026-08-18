namespace CryptoHub.Api.Models;

public record CandleResponse
{
    public required TradingPair TradingPair { get; init; }
    public string Interval { get; init; } = string.Empty;
    public required IReadOnlyList<Candle> Candles { get; init; }
}