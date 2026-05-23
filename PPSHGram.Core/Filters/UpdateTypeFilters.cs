using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

public sealed class MessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContext);

    public override bool Matches(IContext context) => context.Update.Message is not null;
}

public sealed class EditedMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(EditedMessageContext);

    public override bool Matches(IContext context) => context.Update.EditedMessage is not null;
}

public sealed class ChannelPostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChannelPostContext);

    public override bool Matches(IContext context) => context.Update.ChannelPost is not null;
}

public sealed class EditedChannelPostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(EditedChannelPostContext);

    public override bool Matches(IContext context) => context.Update.EditedChannelPost is not null;
}

public sealed class BusinessConnectionAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(BusinessConnectionContext);

    public override bool Matches(IContext context) => context.Update.BusinessConnection is not null;
}

public sealed class BusinessMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(BusinessMessageContext);

    public override bool Matches(IContext context) => context.Update.BusinessMessage is not null;
}

public sealed class EditedBusinessMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(EditedBusinessMessageContext);

    public override bool Matches(IContext context) => context.Update.EditedBusinessMessage is not null;
}

public sealed class DeletedBusinessMessagesAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(DeletedBusinessMessagesContext);

    public override bool Matches(IContext context) => context.Update.DeletedBusinessMessages is not null;
}

public sealed class GuestMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(GuestMessageContext);

    public override bool Matches(IContext context) => context.Update.GuestMessage is not null;
}

public sealed class MessageReactionAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageReactionContext);

    public override bool Matches(IContext context) => context.Update.MessageReaction is not null;
}

public sealed class MessageReactionCountAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageReactionCountContext);

    public override bool Matches(IContext context) => context.Update.MessageReactionCount is not null;
}

public sealed class CallbackQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(CallbackQueryContext);

    public override bool Matches(IContext context) => context.Update.CallbackQuery is not null;
}

public sealed class InlineQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(InlineQueryContext);

    public override bool Matches(IContext context) => context.Update.InlineQuery is not null;
}

public sealed class ChosenInlineResultAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChosenInlineResultContext);

    public override bool Matches(IContext context) => context.Update.ChosenInlineResult is not null;
}

public sealed class ShippingQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ShippingQueryContext);

    public override bool Matches(IContext context) => context.Update.ShippingQuery is not null;
}

public sealed class PreCheckoutQueryAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PreCheckoutQueryContext);

    public override bool Matches(IContext context) => context.Update.PreCheckoutQuery is not null;
}

public sealed class PurchasedPaidMediaAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PaidMediaPurchasedContext);

    public override bool Matches(IContext context) => context.Update.PurchasedPaidMedia is not null;
}

public sealed class PollUpdateAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PollContext);

    public override bool Matches(IContext context) => context.Update.Poll is not null;
}

public sealed class PollAnswerAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(PollAnswerContext);

    public override bool Matches(IContext context) => context.Update.PollAnswer is not null;
}

public sealed class ChatMemberAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChatMemberContext);

    public override bool Matches(IContext context) => context.Update.ChatMember is not null;
}

public sealed class MyChatMemberAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MyChatMemberContext);

    public override bool Matches(IContext context) => context.Update.MyChatMember is not null;
}

public sealed class ChatJoinRequestAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChatJoinRequestContext);

    public override bool Matches(IContext context) => context.Update.ChatJoinRequest is not null;
}

public sealed class ChatBoostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ChatBoostContext);

    public override bool Matches(IContext context) => context.Update.ChatBoost is not null;
}

public sealed class RemovedChatBoostAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(RemovedChatBoostContext);

    public override bool Matches(IContext context) => context.Update.RemovedChatBoost is not null;
}

public sealed class ManagedBotAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(ManagedBotContext);

    public override bool Matches(IContext context) => context.Update.ManagedBot is not null;
}
