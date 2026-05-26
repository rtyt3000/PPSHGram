using System.Text.Json.Serialization;
using PPSHGram.Telegram.Json;

namespace PPSHGram.Telegram.Generated.Types;

[JsonConverter(typeof(InputFileJsonConverter))]
public partial class InputFile
{
    private InputFile(InputFileKind kind, string? value, byte[]? bytes, Stream? stream, string? filename, string? contentType)
    {
        Kind = kind;
        Value = value;
        Bytes = bytes;
        Stream = stream;
        Filename = filename;
        ContentType = contentType;
    }

    internal InputFileKind Kind { get; }

    internal string? Value { get; }

    internal byte[]? Bytes { get; }

    internal Stream? Stream { get; }

    internal string? Filename { get; }

    internal string? ContentType { get; }

    internal bool RequiresUpload => Kind is InputFileKind.Path or InputFileKind.Bytes or InputFileKind.Stream;

    public static InputFile FromFileId(string fileId) => FromString(fileId);

    public static InputFile FromUrl(string url) => FromString(url);

    public static InputFile FromUrl(Uri url) => FromString(url.AbsoluteUri);

    public static InputFile FromPath(string path, string? filename = null, string? contentType = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return new InputFile(
            InputFileKind.Path,
            path,
            bytes: null,
            stream: null,
            filename ?? Path.GetFileName(path),
            contentType);
    }

    public static InputFile FromBytes(byte[] bytes, string filename, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);
        return new InputFile(InputFileKind.Bytes, value: null, bytes, stream: null, filename, contentType);
    }

    public static InputFile FromStream(Stream stream, string filename, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);
        return new InputFile(InputFileKind.Stream, value: null, bytes: null, stream, filename, contentType);
    }

    public static implicit operator InputFile(string value) => FromString(value);

    private static InputFile FromString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new InputFile(InputFileKind.String, value, bytes: null, stream: null, filename: null, contentType: null);
    }
}

internal enum InputFileKind
{
    String,
    Path,
    Bytes,
    Stream
}
