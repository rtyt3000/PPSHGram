using PPSHGram.Telegram.Generated.Requests;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core.Models.Context;

public class Context(Api api, Update update) : IContext
{
    public Api Api { get; } = api;

    public Update Update { get; } = update;
}

public abstract class MessageContextBase(Api api, Update update, Message message) : Context(api, update)
{
    public Message Message { get; } = message;

    public Chat Chat => Message.Chat;

    public object ChatId => Message.Chat.Id;

    public long MessageId => Message.MessageId;

    public User? From => Message.From;

    public string? Text => Message.Text;

    public Task<Message> Answer(
        string text,
        string? parseMode = null,
        ReplyMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
    {
        return Api.SendMessage(CreateSendMessageRequest(text, parseMode, replyMarkup, replyParameters: null), cancellationToken);
    }

    public Task<Message> Reply(
        string text,
        string? parseMode = null,
        ReplyMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
    {
        var replyParameters = new ReplyParameters
        {
            MessageId = Message.MessageId
        };

        return Api.SendMessage(CreateSendMessageRequest(text, parseMode, replyMarkup, replyParameters), cancellationToken);
    }

    public Task<bool> Delete(CancellationToken cancellationToken = default)
    {
        return Api.DeleteMessage(ChatId, MessageId, cancellationToken);
    }

    public Task<bool> EditText(
        string text,
        string? parseMode = null,
        InlineKeyboardMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
    {
        return Api.EditMessageText(new EditMessageTextRequest
        {
            BusinessConnectionId = Message.BusinessConnectionId,
            ChatId = ChatId,
            MessageId = MessageId,
            Text = text,
            ParseMode = parseMode,
            ReplyMarkup = replyMarkup
        }, cancellationToken);
    }

    private SendMessageRequest CreateSendMessageRequest(
        string text,
        string? parseMode,
        ReplyMarkup? replyMarkup,
        ReplyParameters? replyParameters)
    {
        return new SendMessageRequest
        {
            BusinessConnectionId = Message.BusinessConnectionId,
            ChatId = ChatId,
            MessageThreadId = Message.MessageThreadId,
            DirectMessagesTopicId = Message.DirectMessagesTopic?.TopicId,
            Text = text,
            ParseMode = parseMode,
            ReplyMarkup = replyMarkup,
            ReplyParameters = replyParameters
        };
    }
}

public class MessageContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

public sealed class CommandContext : MessageContext
{
    public CommandContext(Api api, Update update, Message message) : base(api, update, message)
    {
        if (!TryParse(message.Text, out var command))
        {
            throw new ArgumentException("Message text must start with a bot command.", nameof(message));
        }

        Command = command.Command;
        ArgumentText = command.ArgumentText;
        Arguments = command.Arguments;
    }

    public string Command { get; }

    public string ArgumentText { get; }

    public IReadOnlyList<string> Arguments { get; }

    internal static bool TryParse(string? text, out CommandData command)
    {
        command = default;
        if (string.IsNullOrWhiteSpace(text) || text[0] != '/')
        {
            return false;
        }

        var commandEnd = text.IndexOf(' ');
        var rawCommand = commandEnd < 0 ? text[1..] : text[1..commandEnd];
        if (string.IsNullOrWhiteSpace(rawCommand))
        {
            return false;
        }

        var mentionStart = rawCommand.IndexOf('@');
        var commandName = mentionStart < 0 ? rawCommand : rawCommand[..mentionStart];
        if (string.IsNullOrWhiteSpace(commandName))
        {
            return false;
        }

        var argumentText = commandEnd < 0 ? string.Empty : text[(commandEnd + 1)..].Trim();
        var arguments = argumentText.Length == 0
            ? []
            : argumentText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        command = new CommandData(commandName, argumentText, arguments);
        return true;
    }

    internal readonly record struct CommandData(
        string Command,
        string ArgumentText,
        IReadOnlyList<string> Arguments);
}

public sealed class EditedMessageContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

public sealed class ChannelPostContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

public sealed class EditedChannelPostContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

public sealed class BusinessMessageContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

public sealed class EditedBusinessMessageContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

public sealed class GuestMessageContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

public sealed class BusinessConnectionContext(Api api, Update update, BusinessConnection businessConnection) : Context(api, update)
{
    public BusinessConnection BusinessConnection { get; } = businessConnection;
}

public sealed class DeletedBusinessMessagesContext(Api api, Update update, BusinessMessagesDeleted deletedBusinessMessages) : Context(api, update)
{
    public BusinessMessagesDeleted DeletedBusinessMessages { get; } = deletedBusinessMessages;
}

public sealed class MessageReactionContext(Api api, Update update, MessageReactionUpdated messageReaction) : Context(api, update)
{
    public MessageReactionUpdated MessageReaction { get; } = messageReaction;
}

public sealed class MessageReactionCountContext(Api api, Update update, MessageReactionCountUpdated messageReactionCount) : Context(api, update)
{
    public MessageReactionCountUpdated MessageReactionCount { get; } = messageReactionCount;
}

public sealed class InlineQueryContext(Api api, Update update, InlineQuery inlineQuery) : Context(api, update)
{
    public InlineQuery InlineQuery { get; } = inlineQuery;

    public Task<bool> Answer(
        IReadOnlyList<InlineQueryResult> results,
        long? cacheTime = null,
        bool? isPersonal = null,
        string? nextOffset = null,
        InlineQueryResultsButton? button = null,
        CancellationToken cancellationToken = default)
    {
        return Api.AnswerInlineQuery(new AnswerInlineQueryRequest
        {
            InlineQueryId = InlineQuery.Id,
            Results = results,
            CacheTime = cacheTime,
            IsPersonal = isPersonal,
            NextOffset = nextOffset,
            Button = button
        }, cancellationToken);
    }
}

public sealed class ChosenInlineResultContext(Api api, Update update, ChosenInlineResult chosenInlineResult) : Context(api, update)
{
    public ChosenInlineResult ChosenInlineResult { get; } = chosenInlineResult;
}

public sealed class CallbackQueryContext(Api api, Update update, CallbackQuery callbackQuery) : Context(api, update)
{
    public CallbackQuery CallbackQuery { get; } = callbackQuery;

    public string? Data => CallbackQuery.Data;

    public Task<bool> Answer(
        string? text = null,
        bool? showAlert = null,
        string? url = null,
        long? cacheTime = null,
        CancellationToken cancellationToken = default)
    {
        return Api.AnswerCallbackQuery(new AnswerCallbackQueryRequest
        {
            CallbackQueryId = CallbackQuery.Id,
            Text = text,
            ShowAlert = showAlert,
            Url = url,
            CacheTime = cacheTime
        }, cancellationToken);
    }
}

public sealed class ShippingQueryContext(Api api, Update update, ShippingQuery shippingQuery) : Context(api, update)
{
    public ShippingQuery ShippingQuery { get; } = shippingQuery;

    public Task<bool> Answer(
        bool ok,
        IReadOnlyList<ShippingOption>? shippingOptions = null,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        return Api.AnswerShippingQuery(new AnswerShippingQueryRequest
        {
            ShippingQueryId = ShippingQuery.Id,
            Ok = ok,
            ShippingOptions = shippingOptions,
            ErrorMessage = errorMessage
        }, cancellationToken);
    }
}

public sealed class PreCheckoutQueryContext(Api api, Update update, PreCheckoutQuery preCheckoutQuery) : Context(api, update)
{
    public PreCheckoutQuery PreCheckoutQuery { get; } = preCheckoutQuery;

    public Task<bool> Answer(
        bool ok,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        return Api.AnswerPreCheckoutQuery(new AnswerPreCheckoutQueryRequest
        {
            PreCheckoutQueryId = PreCheckoutQuery.Id,
            Ok = ok,
            ErrorMessage = errorMessage
        }, cancellationToken);
    }
}

public sealed class PaidMediaPurchasedContext(Api api, Update update, PaidMediaPurchased purchasedPaidMedia) : Context(api, update)
{
    public PaidMediaPurchased PurchasedPaidMedia { get; } = purchasedPaidMedia;
}

public sealed class PollContext(Api api, Update update, Poll poll) : Context(api, update)
{
    public Poll Poll { get; } = poll;
}

public sealed class PollAnswerContext(Api api, Update update, PollAnswer pollAnswer) : Context(api, update)
{
    public PollAnswer PollAnswer { get; } = pollAnswer;
}

public sealed class MyChatMemberContext(Api api, Update update, ChatMemberUpdated myChatMember) : Context(api, update)
{
    public ChatMemberUpdated MyChatMember { get; } = myChatMember;
}

public sealed class ChatMemberContext(Api api, Update update, ChatMemberUpdated chatMember) : Context(api, update)
{
    public ChatMemberUpdated ChatMember { get; } = chatMember;
}

public sealed class ChatJoinRequestContext(Api api, Update update, ChatJoinRequest chatJoinRequest) : Context(api, update)
{
    public ChatJoinRequest ChatJoinRequest { get; } = chatJoinRequest;
}

public sealed class ChatBoostContext(Api api, Update update, ChatBoostUpdated chatBoost) : Context(api, update)
{
    public ChatBoostUpdated ChatBoost { get; } = chatBoost;
}

public sealed class RemovedChatBoostContext(Api api, Update update, ChatBoostRemoved removedChatBoost) : Context(api, update)
{
    public ChatBoostRemoved RemovedChatBoost { get; } = removedChatBoost;
}

public sealed class ManagedBotContext(Api api, Update update, ManagedBotUpdated managedBot) : Context(api, update)
{
    public ManagedBotUpdated ManagedBot { get; } = managedBot;
}
