using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using PPSHGram.Core.Models.Context;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Tests.Context;

public class ContextShortcutTests
{
    [Fact]
    public async Task Message_reply_sends_message_to_current_chat_with_reply_parameters()
    {
        using var api = CreateApi(out var handler, """
            {"ok":true,"result":{"message_id":10,"date":1,"chat":{"id":42,"type":"private"},"text":"pong"}}
            """);
        var message = new Message
        {
            MessageId = 5,
            Date = 1,
            Chat = new Chat { Id = 42, Type = "private" },
            MessageThreadId = 7
        };
        var context = new MessageContext(api, new Update { UpdateId = 1, Message = message }, message);

        await context.Reply("pong", parseMode: "Markdown");

        handler.RequestUri.Should().EndWith("/bottoken/sendMessage");
        using var body = JsonDocument.Parse(handler.RequestBody);
        body.RootElement.GetProperty("chat_id").GetInt64().Should().Be(42);
        body.RootElement.GetProperty("message_thread_id").GetInt64().Should().Be(7);
        body.RootElement.GetProperty("text").GetString().Should().Be("pong");
        body.RootElement.GetProperty("parse_mode").GetString().Should().Be("Markdown");
        body.RootElement.GetProperty("reply_parameters").GetProperty("message_id").GetInt64().Should().Be(5);
    }

    [Fact]
    public async Task Message_answer_sends_message_to_current_chat_without_reply_parameters()
    {
        using var api = CreateApi(out var handler, """
            {"ok":true,"result":{"message_id":10,"date":1,"chat":{"id":42,"type":"private"},"text":"pong"}}
            """);
        var message = new Message
        {
            MessageId = 5,
            Date = 1,
            Chat = new Chat { Id = 42, Type = "private" }
        };
        var context = new MessageContext(api, new Update { UpdateId = 1, Message = message }, message);

        await context.Answer("pong");

        using var body = JsonDocument.Parse(handler.RequestBody);
        body.RootElement.GetProperty("chat_id").GetInt64().Should().Be(42);
        body.RootElement.GetProperty("text").GetString().Should().Be("pong");
        body.RootElement.TryGetProperty("reply_parameters", out _).Should().BeFalse();
    }

    [Fact]
    public async Task Callback_query_answer_uses_current_callback_query_id()
    {
        using var api = CreateApi(out var handler, """{"ok":true,"result":true}""");
        var callbackQuery = new CallbackQuery
        {
            Id = "callback-id",
            From = new User { Id = 100, FirstName = "Tester" },
            ChatInstance = "chat-instance",
            Data = "button:data"
        };
        var context = new CallbackQueryContext(
            api,
            new Update { UpdateId = 1, CallbackQuery = callbackQuery },
            callbackQuery);

        await context.Answer("done", showAlert: true);

        handler.RequestUri.Should().EndWith("/bottoken/answerCallbackQuery");
        using var body = JsonDocument.Parse(handler.RequestBody);
        body.RootElement.GetProperty("callback_query_id").GetString().Should().Be("callback-id");
        body.RootElement.GetProperty("text").GetString().Should().Be("done");
        body.RootElement.GetProperty("show_alert").GetBoolean().Should().BeTrue();
    }

    private static Api CreateApi(out CapturingHandler handler, string responseJson)
    {
        handler = new CapturingHandler(responseJson);
        return new Api("token", new HttpClient(handler), new Uri("https://telegram.test/"));
    }

    private sealed class CapturingHandler(string responseJson) : HttpMessageHandler
    {
        public string RequestUri { get; private set; } = string.Empty;

        public string RequestBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri?.AbsolutePath ?? string.Empty;
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
