using System.Text.Json.Serialization;

namespace PPSHGram.Telegram.CodeGen.Schema;

internal sealed record TelegramSchema(
    string SourceUrl,
    DateTimeOffset GeneratedAt,
    string? BotApiVersion,
    IReadOnlyList<TelegramType> Types,
    IReadOnlyList<TelegramMethod> Methods);

internal sealed record TelegramType(
    string Name,
    string Description,
    IReadOnlyList<TelegramField> Fields);

internal sealed record TelegramMethod(
    string Name,
    string Description,
    string ReturnType,
    IReadOnlyList<TelegramField> Parameters);

internal sealed record TelegramField(
    string Name,
    string Type,
    bool Required,
    string Description);

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TelegramSchema))]
internal sealed partial class SchemaJsonContext : JsonSerializerContext;
