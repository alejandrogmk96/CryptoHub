using CryptoHub.Api.Models;

namespace CryptoHub.Api.Contracts;

public interface IExchange
{
    Task<Price> GetPriceAsync(TradingPair tradingPair);
}