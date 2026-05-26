using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Requests;
using PPSHGram.Telegram.Generated.Types;
using PPSHGram.Telegram.Json;

namespace PPSHGram.Tests.Telegram;

public class InputFileTests
{
    [Fact]
    public void InputFile_string_values_serialize_as_json_strings()
    {
        var json = JsonSerializer.Serialize(InputFile.FromFileId("telegram-file-id"), TelegramJsonSerializerOptions.Default);

        json.Should().Be("\"telegram-file-id\"");
    }

    [Fact]
    public async Task Upload_file_request_is_sent_as_multipart()
    {
        using var api = CreateApi(out var handler, """
            {"ok":true,"result":{"message_id":10,"date":1,"chat":{"id":42,"type":"private"}}}
            """);

        await api.SendPhoto(42, InputFile.FromBytes([1, 2, 3], "photo.jpg", "image/jpeg"));

        handler.RequestUri.Should().EndWith("/bottoken/sendPhoto");
        handler.ContentType.Should().StartWith("multipart/form-data");
        handler.RequestBody.Should().Contain("name=chat_id");
        handler.RequestBody.Should().Contain("42");
        handler.RequestBody.Should().Contain("name=photo");
        handler.RequestBody.Should().Contain("attach://file0");
        handler.RequestBody.Should().Contain("name=file0; filename=photo.jpg");
        handler.RequestBody.Should().Contain("Content-Type: image/jpeg");
    }

    [Fact]
    public async Task Nested_upload_files_are_attached_and_referenced_from_json_payload()
    {
        using var api = CreateApi(out var handler, """{"ok":true,"result":[]}""");

        await api.SendMediaGroup(
            42,
            [
                new InputMediaPhoto
                {
                    Media = InputFile.FromBytes([1], "first.jpg"),
                    Caption = "first"
                },
                new InputMediaPhoto
                {
                    Media = InputFile.FromBytes([2], "second.jpg"),
                    Caption = "second"
                }
            ]);

        handler.ContentType.Should().StartWith("multipart/form-data");
        handler.RequestBody.Should().Contain("name=media");
        handler.RequestBody.Should().Contain("\"media\":\"attach://file0\"");
        handler.RequestBody.Should().Contain("\"media\":\"attach://file1\"");
        handler.RequestBody.Should().Contain("name=file0; filename=first.jpg");
        handler.RequestBody.Should().Contain("name=file1; filename=second.jpg");
    }

    private static Api CreateApi(out CapturingHandler handler, string responseJson)
    {
        handler = new CapturingHandler(responseJson);
        return new Api("token", new HttpClient(handler), new Uri("https://telegram.test/"));
    }

    private sealed class CapturingHandler(string responseJson) : HttpMessageHandler
    {
        public string RequestUri { get; private set; } = string.Empty;

        public string ContentType { get; private set; } = string.Empty;

        public string RequestBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri?.AbsolutePath ?? string.Empty;
            ContentType = request.Content?.Headers.ContentType?.ToString() ?? string.Empty;
            RequestBody = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        }
    }
}
