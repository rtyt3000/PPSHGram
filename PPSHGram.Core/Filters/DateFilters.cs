using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Filters;

public sealed class UpdateDateAttribute : HandlerFilterAttribute
{
    public UpdateDateAttribute(long fromUnixTime = long.MinValue, long toUnixTime = long.MaxValue)
    {
        FromUnixTime = fromUnixTime;
        ToUnixTime = toUnixTime;
    }

    public long FromUnixTime { get; }

    public long ToUnixTime { get; }

    public override bool Matches(IContext context)
    {
        var date = ContextFilterData.GetUnixDate(context);
        return date is not null && date >= FromUnixTime && date <= ToUnixTime;
    }
}

public sealed class AfterDateAttribute : HandlerFilterAttribute
{
    public AfterDateAttribute(long unixTime)
    {
        UnixTime = unixTime;
    }

    public long UnixTime { get; }

    public override bool Matches(IContext context) => ContextFilterData.GetUnixDate(context) >= UnixTime;
}

public sealed class BeforeDateAttribute : HandlerFilterAttribute
{
    public BeforeDateAttribute(long unixTime)
    {
        UnixTime = unixTime;
    }

    public long UnixTime { get; }

    public override bool Matches(IContext context) => ContextFilterData.GetUnixDate(context) <= UnixTime;
}
