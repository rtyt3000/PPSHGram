using System.Net.Http.Json;
using System.Text.Json;
using PPSHGram.Telegram.Json;

namespace PPSHGram.Telegram.Client;

public abstract class TelegramApiClient : IDisposable
{
    public static readonly Uri DefaultBaseAddress = new("https://api.telegram.org/");

    private readonly bool _ownsHttpClient;
    private bool _disposed;

    protected TelegramApiClient(
        string token,
        HttpClient? httpClient = null,
        Uri? baseAddress = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Telegram bot token must not be empty.", nameof(token));
        }

        Token = token;
        HttpClient = httpClient ?? new HttpClient();
        _ownsHttpClient = httpClient is null;
        BaseAddress = EnsureTrailingSlash(baseAddress ?? DefaultBaseAddress);
        JsonSerializerOptions = jsonSerializerOptions ?? TelegramJsonSerializerOptions.Default;
    }

    public string Token { get; }

    public Uri BaseAddress { get; }

    protected HttpClient HttpClient { get; }

    protected JsonSerializerOptions JsonSerializerOptions { get; }

    public async Task<TResult> CallAsync<TResult>(
        string method,
        object? request = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (string.IsNullOrWhiteSpace(method))
        {
            throw new ArgumentException("Telegram Bot API method name must not be empty.", nameof(method));
        }

        var requestPayload = request ?? EmptyRequest.Instance;
        using var content = TelegramMultipartRequestContent.TryCreate(requestPayload, JsonSerializerOptions)
            ?? JsonContent.Create(requestPayload, options: JsonSerializerOptions);
        using var response = await HttpClient.PostAsync(CreateMethodUri(method), content, cancellationToken).ConfigureAwait(false);

        var envelope = await response.Content
            .ReadFromJsonAsync<TelegramApiResponse<TResult>>(JsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        if (envelope is null)
        {
            throw new InvalidOperationException($"Telegram Bot API returned an empty or invalid response for {method}.");
        }

        if (!envelope.Ok)
        {
            throw new TelegramApiException(
                method,
                response.StatusCode,
                envelope.ErrorCode,
                envelope.Description,
                envelope.Parameters);
        }

        if (envelope.Result is null)
        {
            throw new InvalidOperationException($"Telegram Bot API returned a successful response without result for {method}.");
        }

        return envelope.Result;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_ownsHttpClient)
        {
            HttpClient.Dispose();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private Uri CreateMethodUri(string method)
    {
        return new Uri(BaseAddress, $"./bot{Token}/{method}");
    }

    private static Uri EnsureTrailingSlash(Uri uri)
    {
        if (!uri.IsAbsoluteUri)
        {
            throw new ArgumentException("Telegram Bot API base address must be absolute.", nameof(uri));
        }

        return uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
            ? uri
            : new Uri($"{uri.AbsoluteUri}/");
    }

    private sealed class EmptyRequest
    {
        public static readonly EmptyRequest Instance = new();

        private EmptyRequest()
        {
        }
    }
}
