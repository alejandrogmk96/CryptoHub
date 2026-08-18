using CryptoHub.Api.Models;

namespace CryptoHub.Api.Contracts;

public interface IExchange
{
    Task<Price> GetPriceAsync(TradingPair tradingPair);

    Task<IReadOnlyList<Candle>> GetCandlesAsync(
        TradingPair tradingPair,
        CandleInterval interval,
        int limit);
}