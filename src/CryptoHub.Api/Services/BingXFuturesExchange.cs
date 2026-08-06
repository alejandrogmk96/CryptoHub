using System.Net.Http;
using CryptoHub.Api.Contracts;
using CryptoHub.Api.Models;

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

        throw new NotImplementedException();
    }
}