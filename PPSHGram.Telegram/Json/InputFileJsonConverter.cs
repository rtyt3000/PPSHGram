using System.Text.Json;
using System.Text.Json.Serialization;
using PPSHGram.Telegram.Client;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Telegram.Json;

internal sealed class InputFileJsonConverter : JsonConverter<InputFile>
{
    public override InputFile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.String
            ? InputFile.FromFileId(reader.GetString() ?? string.Empty)
            : throw new JsonException("InputFile must be represented as a string in Telegram API JSON.");
    }

    public override void Write(Utf8JsonWriter writer, InputFile value, JsonSerializerOptions options)
    {
        if (!value.RequiresUpload)
        {
            writer.WriteStringValue(value.Value);
            return;
        }

        var multipartContext = TelegramMultipartRequestContext.Current
            ?? throw new InvalidOperationException("InputFile upload values must be sent by TelegramApiClient.");

        writer.WriteStringValue($"attach://{multipartContext.GetOrAddAttachmentName(value)}");
    }
}
