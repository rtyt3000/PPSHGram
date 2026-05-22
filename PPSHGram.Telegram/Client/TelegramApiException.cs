using System.Net;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Telegram.Client;

public sealed class TelegramApiException : Exception
{
    public TelegramApiException(
        string method,
        HttpStatusCode statusCode,
        int? errorCode,
        string? description,
        ResponseParameters? parameters)
        : base(CreateMessage(method, statusCode, errorCode, description))
    {
        Method = method;
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Description = description;
        Parameters = parameters;
    }

    public string Method { get; }

    public HttpStatusCode StatusCode { get; }

    public int? ErrorCode { get; }

    public string? Description { get; }

    public ResponseParameters? Parameters { get; }

    private static string CreateMessage(
        string method,
        HttpStatusCode statusCode,
        int? errorCode,
        string? description)
    {
        var error = errorCode is null ? string.Empty : $" error {errorCode}:";
        var text = string.IsNullOrWhiteSpace(description) ? "Telegram Bot API request failed." : description;
        return $"{method} returned HTTP {(int)statusCode}{error} {text}";
    }
}
