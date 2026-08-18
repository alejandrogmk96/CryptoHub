using System.Text.Json.Serialization;

namespace CryptoHub.Api.Models.External.BingX;

public class BingXPriceResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("data")]
    public BingXPriceData Data { get; set; } = null!;
}