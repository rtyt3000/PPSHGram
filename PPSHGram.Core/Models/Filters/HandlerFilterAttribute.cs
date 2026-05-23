using PPSHGram.Core.Models.Context;

namespace PPSHGram.Core.Models.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class HandlerFilterAttribute : Attribute, IHandlerFilter
{
    public string? Group { get; init; }

    public bool Negate { get; init; }

    public abstract bool Matches(IContext context);
}
