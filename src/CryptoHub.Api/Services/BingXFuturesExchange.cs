using System.Globalization;
using System.Text.Json;
using CryptoHub.Api.Contracts;
using CryptoHub.Api.Models;
using CryptoHub.Api.Models.External.BingX;

namespace CryptoHub.Api.Services;

public class BingXFuturesExchange : IExchange
{
    private const string BaseUrl = "https://open-api.bingx.com";

    private const string PriceEndpoint =
        "/openApi/swap/v2/quote/ticker";

    private const string CandleEndpoint =
        "/openApi/swap/v3/quote/klines";

    private readonly HttpClient _httpClient;

    public BingXFuturesExchange(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Price> GetPriceAsync(
        TradingPair tradingPair)
    {
        var symbol =
            $"{tradingPair.BaseAsset}-{tradingPair.QuoteAsset}";

        var url =
            $"{BaseUrl}{PriceEndpoint}?symbol={symbol}";

        HttpResponseMessage response =
            await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string json =
            await response.Content.ReadAsStringAsync();

        var result =
            JsonSerializer.Deserialize<BingXPriceResponse>(json);

        if (result is null)
        {
            throw new InvalidOperationException(
                "No se pudo deserializar la respuesta de BingX.");
        }

        var price =
            decimal.Parse(
                result.Data.LastPrice,
                CultureInfo.InvariantCulture);

        return new Price(
            tradingPair,
            price,
            DateTimeOffset.UtcNow);
    }

    public async Task<IReadOnlyList<Candle>> GetCandlesAsync(
        TradingPair tradingPair,
        CandleInterval interval,
        int limit)
    {
        var symbol =
            $"{tradingPair.BaseAsset}-{tradingPair.QuoteAsset}";

        var bingXInterval =
            BingXCandleIntervalMapper.ToBingXInterval(interval);

        var url =
            $"{BaseUrl}{CandleEndpoint}" +
            $"?symbol={symbol}" +
            $"&interval={bingXInterval}" +
            $"&limit={limit}";

        HttpResponseMessage response =
            await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string json =
            await response.Content.ReadAsStringAsync();
            Console.WriteLine("RESPUESTA BINGX CANDLES:");
Console.WriteLine(json);

        var result =
            JsonSerializer.Deserialize<BingXCandleResponse>(json);

        if (result is null)
        {
            throw new InvalidOperationException(
                "No se pudo deserializar la respuesta de velas de BingX.");
        }

        if (result.Code != 0)
        {
            throw new InvalidOperationException(
                $"BingX devolvió un error: {result.Msg}");
        }

       var candles = result.Data
          .Select(candle =>
          BingXCandleMapper.ToCandle(
            candle,
            tradingPair))
        .OrderBy(candle => candle.Timestamp)
         .ToList();

    return candles;
    }
}