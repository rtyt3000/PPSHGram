using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

[RequiresContext(typeof(MessageContextBase))]
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

[RequiresContext(typeof(MessageContextBase))]
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

[RequiresContext(typeof(CommandContext))]
public sealed class CommandAttribute : HandlerFilterAttribute
{
    public CommandAttribute(string command)
    {
        Command = command.TrimStart('/');
    }

    public string Command { get; }

    public bool IgnoreCase { get; init; } = true;

    public override Type ContextType => typeof(CommandContext);

    public override bool Matches(IContext context)
    {
        if (!CommandContext.TryParse(ContextFilterData.GetMessage(context)?.Text, out var command))
        {
            return false;
        }

        var comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return string.Equals(command.Command, Command, comparison);
    }
}

[RequiresContext(typeof(CallbackQueryContext))]
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
