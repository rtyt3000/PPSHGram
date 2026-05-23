using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

public sealed class PhotoAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Photo is { Count: > 0 };
}

public sealed class StickerAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Sticker is not null;
}

public sealed class AnimationAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Animation is not null;
}

public sealed class AudioAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Audio is not null;
}

public sealed class DocumentAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Document is not null;
}

public sealed class VideoAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Video is not null;
}

public sealed class VoiceAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Voice is not null;
}

public sealed class ContactAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Contact is not null;
}

public sealed class DiceAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Dice is not null;
}

public sealed class PollMessageAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Poll is not null;
}

public sealed class LocationAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.Location is not null;
}

public sealed class PaymentAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.SuccessfulPayment is not null;
}

public sealed class WebAppDataAttribute : HandlerFilterAttribute
{
    public override bool Matches(IContext context) => ContextFilterData.GetMessage(context)?.WebAppData is not null;
}
