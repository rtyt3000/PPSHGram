using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;
using System.Text.RegularExpressions;

namespace PPSHGram.Core.Filters;

[RequiresContext(typeof(MessageContextBase))]
public sealed class PhotoAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Photo is { Count: > 0 };
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class StickerAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Sticker is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class AnimationAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Animation is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class AudioAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Audio is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class DocumentAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Document is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class VideoAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Video is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class VoiceAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Voice is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class ContactAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Contact is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class DiceAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Dice is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class PollMessageAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Poll is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class LocationAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Location is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class PaymentAttribute : HandlerFilterAttribute
{
    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.SuccessfulPayment is not null;
}

[RequiresContext(typeof(MessageContextBase))]
public sealed class WebAppDataAttribute : HandlerFilterAttribute
{
    public WebAppDataAttribute()
    {
    }

    public WebAppDataAttribute(string value)
    {
        Value = value;
    }

    public WebAppDataAttribute(string value, TextMatchMode mode)
        : this(value)
    {
        Mode = mode;
    }

    public WebAppDataAttribute(Regex regex)
    {
        Regex = regex;
    }

    public string? Value { get; }

    public Regex? Regex { get; }

    public TextMatchMode Mode { get; init; } = TextMatchMode.Exact;

    public bool IgnoreCase { get; init; } = false;

    public override Type ContextType => typeof(MessageContextBase);

    public override bool Matches(IContext context)
    {
        var webAppData = ContextFilterData.GetMessage(context)?.WebAppData;
        if (webAppData is null)
        {
            return false;
        }

        if (Value is null && Regex is null)
        {
            return true;
        }

        return ContextFilterData.MatchesText(
            webAppData.Data,
            Value,
            Mode,
            IgnoreCase,
            Regex);
    }
}
