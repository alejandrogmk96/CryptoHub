namespace CryptoHub.Api.Models;

public record Candle
{
    public required TradingPair TradingPair { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public decimal Volume { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}