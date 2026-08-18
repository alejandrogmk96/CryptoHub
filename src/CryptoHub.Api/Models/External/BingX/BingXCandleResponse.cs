using System.Text.Json.Serialization;

namespace CryptoHub.Api.Models.External.BingX;

public class BingXCandleResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string? Msg { get; set; }

    [JsonPropertyName("data")]
    public List<BingXCandle> Data { get; set; } = [];
}