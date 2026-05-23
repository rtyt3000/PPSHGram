using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

public sealed class TextAttribute : HandlerFilterAttribute
{
    public TextAttribute()
    {
    }

    public TextAttribute(string value)
    {
        Value = value;
    }

    public string? Value { get; }

    public TextMatchMode Mode { get; init; } = TextMatchMode.Exact;

    public bool IgnoreCase { get; init; } = true;

    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context)
    {
        return ContextFilterData.MatchesText(
            ContextFilterData.GetMessage(context)?.Text,
            Value,
            Mode,
            IgnoreCase);
    }
}

public sealed class CaptionAttribute : HandlerFilterAttribute
{
    public CaptionAttribute()
    {
    }

    public CaptionAttribute(string value)
    {
        Value = value;
    }

    public string? Value { get; }

    public TextMatchMode Mode { get; init; } = TextMatchMode.Exact;

    public bool IgnoreCase { get; init; } = true;

    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context)
    {
        return ContextFilterData.MatchesText(
            ContextFilterData.GetMessage(context)?.Caption,
            Value,
            Mode,
            IgnoreCase);
    }
}

public sealed class CommandAttribute : HandlerFilterAttribute
{
    public CommandAttribute(string command)
    {
        Command = command.TrimStart('/');
    }

    public string Command { get; }

    public bool IgnoreCase { get; init; } = true;

    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context)
    {
        var text = ContextFilterData.GetMessage(context)?.Text;
        if (string.IsNullOrWhiteSpace(text) || text[0] != '/')
        {
            return false;
        }

        var commandEnd = text.IndexOf(' ');
        var rawCommand = commandEnd < 0 ? text[1..] : text[1..commandEnd];
        var mentionStart = rawCommand.IndexOf('@');
        var command = mentionStart < 0 ? rawCommand : rawCommand[..mentionStart];
        var comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        return string.Equals(command, Command, comparison);
    }
}

public sealed class CallbackDataAttribute : HandlerFilterAttribute
{
    public CallbackDataAttribute()
    {
    }

    public CallbackDataAttribute(string value)
    {
        Value = value;
    }

    public string? Value { get; }

    public TextMatchMode Mode { get; init; } = TextMatchMode.Exact;

    public bool IgnoreCase { get; init; } = false;

    public override Type ContextType => typeof(CallbackQueryContext);

    public override bool Matches(IContext context)
    {
        return ContextFilterData.MatchesText(
            context.Update.CallbackQuery?.Data,
            Value,
            Mode,
            IgnoreCase);
    }
}
