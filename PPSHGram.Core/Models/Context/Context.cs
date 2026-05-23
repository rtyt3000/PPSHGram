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
}

public sealed class MessageContext(Api api, Update update, Message message) : MessageContextBase(api, update, message);

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
}

public sealed class ChosenInlineResultContext(Api api, Update update, ChosenInlineResult chosenInlineResult) : Context(api, update)
{
    public ChosenInlineResult ChosenInlineResult { get; } = chosenInlineResult;
}

public sealed class CallbackQueryContext(Api api, Update update, CallbackQuery callbackQuery) : Context(api, update)
{
    public CallbackQuery CallbackQuery { get; } = callbackQuery;
}

public sealed class ShippingQueryContext(Api api, Update update, ShippingQuery shippingQuery) : Context(api, update)
{
    public ShippingQuery ShippingQuery { get; } = shippingQuery;
}

public sealed class PreCheckoutQueryContext(Api api, Update update, PreCheckoutQuery preCheckoutQuery) : Context(api, update)
{
    public PreCheckoutQuery PreCheckoutQuery { get; } = preCheckoutQuery;
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
