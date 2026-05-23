using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

public sealed class MessageAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.Message is not null;
}

public sealed class EditedMessageAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.EditedMessage is not null;
}

public sealed class ChannelPostAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.ChannelPost is not null;
}

public sealed class CallbackQueryAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.CallbackQuery is not null;
}

public sealed class InlineQueryAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.InlineQuery is not null;
}

public sealed class ChosenInlineResultAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.ChosenInlineResult is not null;
}

public sealed class ShippingQueryAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.ShippingQuery is not null;
}

public sealed class PreCheckoutQueryAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.PreCheckoutQuery is not null;
}

public sealed class PollUpdateAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.Poll is not null;
}

public sealed class PollAnswerAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.PollAnswer is not null;
}

public sealed class ChatMemberAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.ChatMember is not null;
}

public sealed class MyChatMemberAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.MyChatMember is not null;
}

public sealed class ChatJoinRequestAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => context.Update.ChatJoinRequest is not null;
}
