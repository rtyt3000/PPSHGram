using System.Text.Json;
using System.Text.Json.Serialization;

namespace PPSHGram.Telegram.Json;

public static class TelegramJsonSerializerOptions
{
    public static JsonSerializerOptions Default { get; } = CreateDefault();

    private static JsonSerializerOptions CreateDefault()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = false
        };
    }
}
