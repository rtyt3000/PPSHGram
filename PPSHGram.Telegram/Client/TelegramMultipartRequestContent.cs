using System.Collections;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Telegram.Client;

internal sealed class TelegramMultipartRequestContext
{
    private static readonly AsyncLocal<TelegramMultipartRequestContext?> CurrentContext = new();
    private readonly Dictionary<InputFile, TelegramMultipartAttachment> _attachments = new(ReferenceEqualityComparer.Instance);

    public static TelegramMultipartRequestContext? Current => CurrentContext.Value;

    public IReadOnlyCollection<TelegramMultipartAttachment> Attachments => _attachments.Values;

    public static IDisposable Enter(TelegramMultipartRequestContext context)
    {
        var previous = CurrentContext.Value;
        CurrentContext.Value = context;
        return new Scope(previous);
    }

    public string GetOrAddAttachmentName(InputFile file)
    {
        if (_attachments.TryGetValue(file, out var attachment))
        {
            return attachment.Name;
        }

        var name = $"file{_attachments.Count}";
        _attachments.Add(file, new TelegramMultipartAttachment(name, file));
        return name;
    }

    private sealed class Scope(TelegramMultipartRequestContext? previous) : IDisposable
    {
        public void Dispose()
        {
            CurrentContext.Value = previous;
        }
    }
}

internal static class TelegramMultipartRequestContent
{
    public static HttpContent? TryCreate(object request, JsonSerializerOptions jsonSerializerOptions)
    {
        if (!ContainsUploadFile(request))
        {
            return null;
        }

        var context = new TelegramMultipartRequestContext();
        JsonElement payload;

        using (TelegramMultipartRequestContext.Enter(context))
        {
            payload = JsonSerializer.SerializeToElement(request, request.GetType(), jsonSerializerOptions);
        }

        var content = new MultipartFormDataContent();
        foreach (var property in payload.EnumerateObject())
        {
            if (property.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                continue;
            }

            content.Add(new StringContent(ToFormValue(property.Value), Encoding.UTF8), property.Name);
        }

        foreach (var attachment in context.Attachments)
        {
            content.Add(CreateFileContent(attachment.File), attachment.Name, attachment.GetFilename());
        }

        return content;
    }

    private static bool ContainsUploadFile(object? value)
    {
        return ContainsUploadFile(value, new HashSet<object>(ReferenceEqualityComparer.Instance));
    }

    private static bool ContainsUploadFile(object? value, HashSet<object> visited)
    {
        if (value is null)
        {
            return false;
        }

        if (value is InputFile inputFile)
        {
            return inputFile.RequiresUpload;
        }

        var type = value.GetType();
        if (type.IsPrimitive || value is string or decimal or DateTime or DateTimeOffset or Guid or Uri)
        {
            return false;
        }

        if (!visited.Add(value))
        {
            return false;
        }

        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (ContainsUploadFile(item, visited))
                {
                    return true;
                }
            }

            return false;
        }

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            if (ContainsUploadFile(property.GetValue(value), visited))
            {
                return true;
            }
        }

        return false;
    }

    private static string ToFormValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => value.GetRawText()
        };
    }

    private static HttpContent CreateFileContent(InputFile file)
    {
        HttpContent content = file.Kind switch
        {
            InputFileKind.Path => new StreamContent(System.IO.File.OpenRead(file.Value!)),
            InputFileKind.Bytes => new ByteArrayContent(file.Bytes!),
            InputFileKind.Stream => new StreamContent(file.Stream!),
            _ => throw new InvalidOperationException("Only upload InputFile values can be attached to multipart requests.")
        };

        if (!string.IsNullOrWhiteSpace(file.ContentType))
        {
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        }

        return content;
    }
}

internal sealed record TelegramMultipartAttachment(string Name, InputFile File)
{
    public string GetFilename() => string.IsNullOrWhiteSpace(File.Filename) ? Name : File.Filename!;
}
