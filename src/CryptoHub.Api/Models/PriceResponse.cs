namespace CryptoHub.Api.Models;

public record PriceResponse(
    TradingPair TradingPair,
    decimal Value,
    DateTimeOffset Timestamp);