namespace CryptoHub.Api.Models;

public class TradingPairParser
{
  public TradingPair Parse(string symbol)
{
    symbol = symbol.Trim().ToUpperInvariant();

    var parts = symbol.Split('-');

    if (parts.Length != 2)
    {
        throw new ArgumentException(
            "El TradingPair debe tener el formato BASE-QUOTE.",
            nameof(symbol));
    }

    if (string.IsNullOrWhiteSpace(parts[0]) ||
        string.IsNullOrWhiteSpace(parts[1]))
    {
        throw new ArgumentException(
            "El TradingPair debe contener un BaseAsset y un QuoteAsset.",
            nameof(symbol));
    }

    return new TradingPair(
        parts[0],
        parts[1]);
}
}