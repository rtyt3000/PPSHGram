using PPSHGram.Core.Models.Context;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core.Filters;

internal static class ContextFilterData
{
    public static Message? GetMessage(IContext context)
    {
        var update = context.Update;
        return update.Message
            ?? update.EditedMessage
            ?? update.ChannelPost
            ?? update.EditedChannelPost
            ?? update.BusinessMessage
            ?? update.EditedBusinessMessage
            ?? update.GuestMessage;
    }

    public static Chat? GetChat(IContext context)
    {
        var update = context.Update;
        return GetMessage(context)?.Chat
            ?? update.MessageReaction?.Chat
            ?? update.MessageReactionCount?.Chat
            ?? update.MyChatMember?.Chat
            ?? update.ChatMember?.Chat
            ?? update.ChatJoinRequest?.Chat
            ?? update.ChatBoost?.Chat
            ?? update.RemovedChatBoost?.Chat;
    }

    public static User? GetUser(IContext context)
    {
        var update = context.Update;
        return GetMessage(context)?.From
            ?? update.CallbackQuery?.From
            ?? update.InlineQuery?.From
            ?? update.ChosenInlineResult?.From
            ?? update.ShippingQuery?.From
            ?? update.PreCheckoutQuery?.From
            ?? update.PurchasedPaidMedia?.From
            ?? update.PollAnswer?.User
            ?? update.MyChatMember?.From
            ?? update.ChatMember?.From
            ?? update.ChatJoinRequest?.From;
    }

    public static long? GetUnixDate(IContext context)
    {
        var update = context.Update;
        return GetMessage(context)?.Date
            ?? update.MessageReaction?.Date
            ?? update.MyChatMember?.Date
            ?? update.ChatMember?.Date
            ?? update.ChatJoinRequest?.Date;
    }

    public static bool MatchesText(string? actual, string? expected, TextMatchMode mode, bool ignoreCase)
    {
        if (expected is null)
        {
            return actual is not null;
        }

        if (actual is null)
        {
            return false;
        }

        var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return mode switch
        {
            TextMatchMode.Exact => string.Equals(actual, expected, comparison),
            TextMatchMode.Contains => actual.Contains(expected, comparison),
            TextMatchMode.StartsWith => actual.StartsWith(expected, comparison),
            TextMatchMode.EndsWith => actual.EndsWith(expected, comparison),
            TextMatchMode.Regex => System.Text.RegularExpressions.Regex.IsMatch(
                actual,
                expected,
                ignoreCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None),
            _ => false
        };
    }
}
