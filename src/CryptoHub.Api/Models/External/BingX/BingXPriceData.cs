using System.Text.Json.Serialization;

namespace CryptoHub.Api.Models.External.BingX;

public class BingXPriceData
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("lastPrice")]
    public string LastPrice { get; set; } = string.Empty;
}