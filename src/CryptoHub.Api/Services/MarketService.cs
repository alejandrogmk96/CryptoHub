using System.Text.Json;

namespace CryptoHub.Api.Services;

public class MarketService
{
    private readonly HttpClient _client;

    public MarketService(HttpClient client)
    {
        _client = client;
    }

    public async Task<object> GetTickerAsync(string symbol)
    {
        var url = $"https://open-api.bingx.com/openApi/swap/v2/quote/ticker?symbol={symbol}";

        var response = await _client.GetAsync(url);

        var json = await response.Content.ReadAsStringAsync();

        var document = JsonDocument.Parse(json);

        var data = document.RootElement.GetProperty("data");

        var lastPrice = data.GetProperty("lastPrice").GetString();

        return new
        {
            symbol,
            price = lastPrice
        };
    }
}