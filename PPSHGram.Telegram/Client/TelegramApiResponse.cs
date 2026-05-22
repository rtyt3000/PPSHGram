using System.Text.Json.Serialization;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Telegram.Client;

internal sealed class TelegramApiResponse<TResult>
{
    [JsonPropertyName("ok")]
    public bool Ok { get; init; }

    [JsonPropertyName("result")]
    public TResult? Result { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("error_code")]
    public int? ErrorCode { get; init; }

    [JsonPropertyName("parameters")]
    public ResponseParameters? Parameters { get; init; }
}
