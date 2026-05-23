using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using PPSHGram.Core;
using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Handlers;
using PPSHGram.Core.Models.Middlewares;
using PPSHGram.Core.Models.Polling;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Tests;

public class BotPipelineTests
{
    [Fact]
    public async Task Handle_update_runs_middlewares_around_direct_handlers()
    {
        var events = new List<string>();
        using var bot = CreateBot();

        bot.UseMiddleware(new RecordingMiddleware(events));
        bot.UseHandler(new RecordingHandler(events));

        await bot.HandleUpdateAsync(CreateMessageUpdate("hello"));

        events.Should().Equal("before", "handler:MessageContext", "after");
    }

    [Fact]
    public async Task Handle_update_discovers_attribute_handlers_and_creates_them_from_services()
    {
        AttributeHandler.Reset();
        var dependency = new TestDependency("service-value");
        using var bot = CreateBot(new TestServiceProvider(dependency));

        bot.UseHandler<AttributeHandler>();

        await bot.HandleUpdateAsync(CreateMessageUpdate("hello world"));
        await bot.HandleUpdateAsync(CreateMessageUpdate("ignored"));

        AttributeHandler.Calls.Should().Be(1);
        AttributeHandler.LastText.Should().Be("hello world");
        AttributeHandler.DependencyValue.Should().Be("service-value");
    }

    [Fact]
    public async Task Start_polling_reads_updates_and_dispatches_them()
    {
        var responses = new Queue<string>([
            """
            {"ok":true,"result":[{"update_id":10,"message":{"message_id":1,"date":1,"chat":{"id":42,"type":"private"},"text":"hello"}}]}
            """
        ]);
        var handler = new CapturingHandler(() => responses.Dequeue());
        using var api = new Api("token", new HttpClient(handler), new Uri("https://telegram.test/"));
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var handled = 0;
        var bot = new Bot(api);

        bot.UseHandler(new DelegateHandler((context, _) =>
        {
            handled++;
            context.Should().BeOfType<MessageContext>();
            cancellation.Cancel();
            return Task.CompletedTask;
        }));

        await bot.StartPolling(new PollingOptions { Timeout = 1, Limit = 10 }, cancellation.Token);

        handled.Should().Be(1);
        handler.RequestUris.Should().Equal("/bottoken/getUpdates");
        using var request = JsonDocument.Parse(handler.RequestBodies.Single());
        request.RootElement.GetProperty("timeout").GetInt64().Should().Be(1);
        request.RootElement.GetProperty("limit").GetInt64().Should().Be(10);
    }

    private static Bot CreateBot(IServiceProvider? serviceProvider = null)
    {
        var handler = new CapturingHandler(() => """{"ok":true,"result":true}""");
        var api = new Api("token", new HttpClient(handler), new Uri("https://telegram.test/"));
        return new Bot(api, serviceProvider);
    }

    private static Update CreateMessageUpdate(string text)
    {
        return new Update
        {
            UpdateId = 1,
            Message = new Message
            {
                MessageId = 1,
                Date = 1,
                Chat = new Chat { Id = 42, Type = "private" },
                Text = text
            }
        };
    }

    private sealed class RecordingMiddleware(List<string> events) : IMiddleware
    {
        public async Task Handle(
            IContext context,
            MiddlewareHandler next,
            CancellationToken cancellationToken = default)
        {
            events.Add("before");
            await next(context, cancellationToken);
            events.Add("after");
        }
    }

    private sealed class RecordingHandler(List<string> events) : IHandler
    {
        public Task Handle(IContext context, CancellationToken cancellationToken = default)
        {
            events.Add($"handler:{context.GetType().Name}");
            return Task.CompletedTask;
        }
    }

    private sealed class DelegateHandler(Func<IContext, CancellationToken, Task> handle) : IHandler
    {
        public Task Handle(IContext context, CancellationToken cancellationToken = default)
        {
            return handle(context, cancellationToken);
        }
    }

    [PPSHGram.Core.Filters.MessageAttribute]
    [PPSHGram.Core.Filters.TextAttribute("hello", Mode = PPSHGram.Core.Filters.TextMatchMode.StartsWith)]
    private sealed class AttributeHandler(TestDependency dependency)
    {
        public static int Calls { get; private set; }

        public static string? LastText { get; private set; }

        public static string? DependencyValue { get; private set; }

        public static void Reset()
        {
            Calls = 0;
            LastText = null;
            DependencyValue = null;
        }

        public ValueTask Handle(MessageContext context, CancellationToken cancellationToken = default)
        {
            Calls++;
            LastText = context.Text;
            DependencyValue = dependency.Value;
            return ValueTask.CompletedTask;
        }
    }

    private sealed record TestDependency(string Value);

    private sealed class TestServiceProvider(params object[] services) : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return services.FirstOrDefault(service => serviceType.IsInstanceOfType(service));
        }
    }

    private sealed class CapturingHandler(Func<string> responseFactory) : HttpMessageHandler
    {
        public List<string> RequestUris { get; } = [];

        public List<string> RequestBodies { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUris.Add(request.RequestUri?.AbsolutePath ?? string.Empty);
            RequestBodies.Add(request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken));

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseFactory(), Encoding.UTF8, "application/json")
            };
        }
    }
}
