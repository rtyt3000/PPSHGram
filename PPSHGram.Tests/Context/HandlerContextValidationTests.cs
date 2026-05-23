using FluentAssertions;
using PPSHGram.Core.Filters;
using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Exeptions;
using PPSHGram.Core.Models.Filters;
using PPSHGram.Core.Models.Handlers;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Tests.Context;

public class HandlerContextValidationTests
{
    [Fact]
    public void Update_context_factory_creates_message_context()
    {
        var update = new Update
        {
            Message = new Message
            {
                MessageId = 1,
                Date = 1,
                Chat = new Chat { Id = 1, Type = "private" }
            }
        };

        var context = UpdateContextFactory.Create(new Api("test-token"), update);

        context.Should().BeOfType<MessageContext>();
    }

    [Fact]
    public void Resolve_context_type_prefers_more_specific_filter_context()
    {
        var method = typeof(ValidMessageHandler).GetMethod(nameof(ValidMessageHandler.Handle))!;

        HandlerFilterEvaluator.ResolveContextType(typeof(ValidMessageHandler), method)
            .Should()
            .Be(typeof(MessageContext));
    }

    [Fact]
    public void Validate_handler_context_accepts_compatible_context_parameter()
    {
        var method = typeof(ValidMessageHandler).GetMethod(nameof(ValidMessageHandler.Handle))!;

        var action = () => HandlerFilterEvaluator.ValidateHandlerContext(typeof(ValidMessageHandler), method);

        action.Should().NotThrow();
    }

    [Fact]
    public void Validate_handler_context_rejects_incompatible_context_parameter()
    {
        var method = typeof(InvalidMessageHandler).GetMethod(nameof(InvalidMessageHandler.Handle))!;

        var action = () => HandlerFilterEvaluator.ValidateHandlerContext(typeof(InvalidMessageHandler), method);

        action.Should().Throw<HandlerContextValidationException>();
    }

    [Fact]
    public void Resolve_context_type_rejects_incompatible_filters()
    {
        var method = typeof(ConflictingHandler).GetMethod(nameof(ConflictingHandler.Handle))!;

        var action = () => HandlerFilterEvaluator.ResolveContextType(typeof(ConflictingHandler), method);

        action.Should().Throw<HandlerContextValidationException>();
    }

    [Fact]
    public void Handler_discovery_rejects_class_based_handler_with_wrong_context()
    {
        var action = () => HandlerDiscovery.Discover(typeof(InvalidClassBasedHandler));

        action.Should().Throw<HandlerContextValidationException>();
    }

    [Message]
    [Text]
    private sealed class ValidMessageHandler
    {
        public Task Handle(MessageContext context) => Task.CompletedTask;
    }

#pragma warning disable PPSHG001, PPSHG003

    [Message]
    private sealed class InvalidMessageHandler
    {
        public Task Handle(CallbackQueryContext context) => Task.CompletedTask;
    }

    [Message]
    [CallbackData]
    private sealed class ConflictingHandler
    {
        public Task Handle(IContext context) => Task.CompletedTask;
    }

    [Message]
    [Text]
    private sealed class InvalidClassBasedHandler
    {
        public Task Handle(CallbackQueryContext context) => Task.CompletedTask;
    }

#pragma warning restore PPSHG001, PPSHG003
}
