using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

public sealed class ChatTypeAttribute : HandlerFilterAttribute
{
    public ChatTypeAttribute(string type)
    {
        Type = type;
    }

    public string Type { get; }

    public override bool Matches(IContext context)
    {
        return string.Equals(ContextFilterData.GetChat(context)?.Type, Type, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class PrivateChatAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ChatTypeFilter.IsChatType(context, "private");
}

public sealed class GroupChatAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ChatTypeFilter.IsChatType(context, "group");
}

public sealed class SupergroupChatAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ChatTypeFilter.IsChatType(context, "supergroup");
}

public sealed class ChannelChatAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ChatTypeFilter.IsChatType(context, "channel");
}

public sealed class FromUserAttribute : HandlerFilterAttribute
{
    public FromUserAttribute(long userId)
    {
        UserId = userId;
    }

    public long UserId { get; }

    public override bool Matches(IContext context) => ContextFilterData.GetUser(context)?.Id == UserId;
}

public sealed class FromUsernameAttribute : HandlerFilterAttribute
{
    public FromUsernameAttribute(string username)
    {
        Username = username.TrimStart('@');
    }

    public string Username { get; }

    public bool IgnoreCase { get; init; } = true;

    public override bool Matches(IContext context)
    {
        var comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return string.Equals(ContextFilterData.GetUser(context)?.Username, Username, comparison);
    }
}

public sealed class FromBotAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetUser(context)?.IsBot == true;
}

public sealed class FromPremiumAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetUser(context)?.IsPremium == true;
}

internal static class ChatTypeFilter
{
    public static bool IsChatType(IContext context, string type)
    {
        return string.Equals(ContextFilterData.GetChat(context)?.Type, type, StringComparison.OrdinalIgnoreCase);
    }
}
