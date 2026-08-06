namespace CryptoHub.Api.Models;

public class Price
{
    public TradingPair TradingPair { get; }
    public decimal Value { get; }
    public DateTimeOffset Timestamp { get; }

    public Price(
        TradingPair tradingPair,
        decimal value,
        DateTimeOffset timestamp)
    {
        ArgumentNullException.ThrowIfNull(tradingPair);

        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "El precio no puede ser negativo.");
        }

        TradingPair = tradingPair;
        Value = value;
        Timestamp = timestamp;
    }
}