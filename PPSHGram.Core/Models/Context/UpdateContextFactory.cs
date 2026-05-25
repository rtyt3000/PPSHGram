using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core.Models.Context;

public static class UpdateContextFactory
{
    public static IContext Create(Api api, Update update)
    {
        ArgumentNullException.ThrowIfNull(api);
        ArgumentNullException.ThrowIfNull(update);

        if (update.Message is not null)
        {
            return CommandContext.TryParse(update.Message.Text, out _)
                ? new CommandContext(api, update, update.Message)
                : new MessageContext(api, update, update.Message);
        }
        if (update.EditedMessage is not null) return new EditedMessageContext(api, update, update.EditedMessage);
        if (update.ChannelPost is not null) return new ChannelPostContext(api, update, update.ChannelPost);
        if (update.EditedChannelPost is not null) return new EditedChannelPostContext(api, update, update.EditedChannelPost);
        if (update.BusinessConnection is not null) return new BusinessConnectionContext(api, update, update.BusinessConnection);
        if (update.BusinessMessage is not null) return new BusinessMessageContext(api, update, update.BusinessMessage);
        if (update.EditedBusinessMessage is not null) return new EditedBusinessMessageContext(api, update, update.EditedBusinessMessage);
        if (update.DeletedBusinessMessages is not null) return new DeletedBusinessMessagesContext(api, update, update.DeletedBusinessMessages);
        if (update.GuestMessage is not null) return new GuestMessageContext(api, update, update.GuestMessage);
        if (update.MessageReaction is not null) return new MessageReactionContext(api, update, update.MessageReaction);
        if (update.MessageReactionCount is not null) return new MessageReactionCountContext(api, update, update.MessageReactionCount);
        if (update.InlineQuery is not null) return new InlineQueryContext(api, update, update.InlineQuery);
        if (update.ChosenInlineResult is not null) return new ChosenInlineResultContext(api, update, update.ChosenInlineResult);
        if (update.CallbackQuery is not null) return new CallbackQueryContext(api, update, update.CallbackQuery);
        if (update.ShippingQuery is not null) return new ShippingQueryContext(api, update, update.ShippingQuery);
        if (update.PreCheckoutQuery is not null) return new PreCheckoutQueryContext(api, update, update.PreCheckoutQuery);
        if (update.PurchasedPaidMedia is not null) return new PaidMediaPurchasedContext(api, update, update.PurchasedPaidMedia);
        if (update.Poll is not null) return new PollContext(api, update, update.Poll);
        if (update.PollAnswer is not null) return new PollAnswerContext(api, update, update.PollAnswer);
        if (update.MyChatMember is not null) return new MyChatMemberContext(api, update, update.MyChatMember);
        if (update.ChatMember is not null) return new ChatMemberContext(api, update, update.ChatMember);
        if (update.ChatJoinRequest is not null) return new ChatJoinRequestContext(api, update, update.ChatJoinRequest);
        if (update.ChatBoost is not null) return new ChatBoostContext(api, update, update.ChatBoost);
        if (update.RemovedChatBoost is not null) return new RemovedChatBoostContext(api, update, update.RemovedChatBoost);
        if (update.ManagedBot is not null) return new ManagedBotContext(api, update, update.ManagedBot);

        return new Context(api, update);
    }
}
