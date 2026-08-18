using System.Net.Http;
using CryptoHub.Api.Contracts;
using CryptoHub.Api.Models;
using System.Text.Json;
using CryptoHub.Api.Models.External.BingX;

namespace CryptoHub.Api.Services;

public class BingXFuturesExchange : IExchange
{
    private const string BaseUrl = "https://open-api.bingx.com";
    private const string PriceEndpoint = "/openApi/swap/v2/quote/ticker";

    private readonly HttpClient _httpClient;

    public BingXFuturesExchange(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Price> GetPriceAsync(TradingPair tradingPair)
    {
        var symbol = $"{tradingPair.BaseAsset}-{tradingPair.QuoteAsset}";

        var url = $"{BaseUrl}{PriceEndpoint}?symbol={symbol}";

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
var value = decimal.Parse(result.Data.LastPrice);

return new Price(
    tradingPair,
    value,
    DateTimeOffset.UtcNow);
    }
}