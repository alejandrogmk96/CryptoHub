using CryptoHub.Api.Contracts;
using CryptoHub.Api.Models;
using CryptoHub.Api.Services;

namespace CryptoHub.Api.Endpoints;

public static class MarketEndpoints
{
    public static void MapMarketEndpoints(this WebApplication app)
    {
        app.MapGet("/api/markets/{symbol}/price", async (
            string symbol,
            IExchange exchange,
            TradingPairParser parser) =>
        {
            try
            {
                var tradingPair = parser.Parse(symbol);

                var price =
                    await exchange.GetPriceAsync(tradingPair);

                return Results.Ok(price);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(
                    new ApiError(
                        "invalid_trading_pair",
                        ex.Message));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        app.MapGet("/api/markets/{symbol}/candles", async (
            string symbol,
            string interval,
            int limit,
            IExchange exchange,
            TradingPairParser parser) =>
        {
            try
            {
                var tradingPair = parser.Parse(symbol);

                var candleInterval =
                    Enum.Parse<CandleInterval>(
                        interval,
                        ignoreCase: true);

                var candles =
                    await exchange.GetCandlesAsync(
                        tradingPair,
                        candleInterval,
                        limit);

                var result = new CandleResponse
                {
                    TradingPair = tradingPair,
                    Interval =
                        CandleIntervalMapper.ToApiValue(
                            candleInterval),
                    Candles = candles
                };

                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(
                    new ApiError(
                        "invalid_request",
                        ex.Message));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}