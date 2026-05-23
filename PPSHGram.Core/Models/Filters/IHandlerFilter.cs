using PPSHGram.Core.Models.Context;

namespace PPSHGram.Core.Models.Filters;

public interface IHandlerFilter
{
    string? Group { get; }

    bool Negate { get; }

    Type ContextType { get; }

    bool Matches(IContext context);
}
