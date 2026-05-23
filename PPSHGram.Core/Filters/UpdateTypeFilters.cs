using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

[RequiresContext(typeof(MessageContext))]
public sealed class MessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContext);

    public override bool Matches(IContext context) => context.Update.Message is not null;
}

[RequiresContext(typeof(EditedMessageContext))]
public sealed class EditedMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(EditedMessageContext);

    public override bool Matches(IContext context) => context.Update.EditedMessage is not null;
}

[RequiresContext(typeof(ChannelPostContext))]
public sealed class ChannelPostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChannelPostContext);

    public override bool Matches(IContext context) => context.Update.ChannelPost is not null;
}

[RequiresContext(typeof(EditedChannelPostContext))]
public sealed class EditedChannelPostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(EditedChannelPostContext);

    public override bool Matches(IContext context) => context.Update.EditedChannelPost is not null;
}

[RequiresContext(typeof(BusinessConnectionContext))]
public sealed class BusinessConnectionAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(BusinessConnectionContext);

    public override bool Matches(IContext context) => context.Update.BusinessConnection is not null;
}

[RequiresContext(typeof(BusinessMessageContext))]
public sealed class BusinessMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(BusinessMessageContext);

    public override bool Matches(IContext context) => context.Update.BusinessMessage is not null;
}

[RequiresContext(typeof(EditedBusinessMessageContext))]
public sealed class EditedBusinessMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(EditedBusinessMessageContext);

    public override bool Matches(IContext context) => context.Update.EditedBusinessMessage is not null;
}

[RequiresContext(typeof(DeletedBusinessMessagesContext))]
public sealed class DeletedBusinessMessagesAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(DeletedBusinessMessagesContext);

    public override bool Matches(IContext context) => context.Update.DeletedBusinessMessages is not null;
}

[RequiresContext(typeof(GuestMessageContext))]
public sealed class GuestMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(GuestMessageContext);

    public override bool Matches(IContext context) => context.Update.GuestMessage is not null;
}

[RequiresContext(typeof(MessageReactionContext))]
public sealed class MessageReactionAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageReactionContext);

    public override bool Matches(IContext context) => context.Update.MessageReaction is not null;
}

[RequiresContext(typeof(MessageReactionCountContext))]
public sealed class MessageReactionCountAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageReactionCountContext);

    public override bool Matches(IContext context) => context.Update.MessageReactionCount is not null;
}

[RequiresContext(typeof(CallbackQueryContext))]
public sealed class CallbackQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(CallbackQueryContext);

    public override bool Matches(IContext context) => context.Update.CallbackQuery is not null;
}

[RequiresContext(typeof(InlineQueryContext))]
public sealed class InlineQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(InlineQueryContext);

    public override bool Matches(IContext context) => context.Update.InlineQuery is not null;
}

[RequiresContext(typeof(ChosenInlineResultContext))]
public sealed class ChosenInlineResultAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChosenInlineResultContext);

    public override bool Matches(IContext context) => context.Update.ChosenInlineResult is not null;
}

[RequiresContext(typeof(ShippingQueryContext))]
public sealed class ShippingQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ShippingQueryContext);

    public override bool Matches(IContext context) => context.Update.ShippingQuery is not null;
}

[RequiresContext(typeof(PreCheckoutQueryContext))]
public sealed class PreCheckoutQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PreCheckoutQueryContext);

    public override bool Matches(IContext context) => context.Update.PreCheckoutQuery is not null;
}

[RequiresContext(typeof(PaidMediaPurchasedContext))]
public sealed class PurchasedPaidMediaAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PaidMediaPurchasedContext);

    public override bool Matches(IContext context) => context.Update.PurchasedPaidMedia is not null;
}

[RequiresContext(typeof(PollContext))]
public sealed class PollUpdateAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PollContext);

    public override bool Matches(IContext context) => context.Update.Poll is not null;
}

[RequiresContext(typeof(PollAnswerContext))]
public sealed class PollAnswerAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PollAnswerContext);

    public override bool Matches(IContext context) => context.Update.PollAnswer is not null;
}

[RequiresContext(typeof(ChatMemberContext))]
public sealed class ChatMemberAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChatMemberContext);

    public override bool Matches(IContext context) => context.Update.ChatMember is not null;
}

[RequiresContext(typeof(MyChatMemberContext))]
public sealed class MyChatMemberAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MyChatMemberContext);

    public override bool Matches(IContext context) => context.Update.MyChatMember is not null;
}

[RequiresContext(typeof(ChatJoinRequestContext))]
public sealed class ChatJoinRequestAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChatJoinRequestContext);

    public override bool Matches(IContext context) => context.Update.ChatJoinRequest is not null;
}

[RequiresContext(typeof(ChatBoostContext))]
public sealed class ChatBoostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChatBoostContext);

    public override bool Matches(IContext context) => context.Update.ChatBoost is not null;
}

[RequiresContext(typeof(RemovedChatBoostContext))]
public sealed class RemovedChatBoostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(RemovedChatBoostContext);

    public override bool Matches(IContext context) => context.Update.RemovedChatBoost is not null;
}

[RequiresContext(typeof(ManagedBotContext))]
public sealed class ManagedBotAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ManagedBotContext);

    public override bool Matches(IContext context) => context.Update.ManagedBot is not null;
}
