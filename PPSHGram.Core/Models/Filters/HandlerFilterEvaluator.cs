using System.Reflection;
using PPSHGram.Core.Models.Context;

namespace PPSHGram.Core.Models.Filters;

public static class HandlerFilterEvaluator
{
    public static bool Matches(IContext context, IEnumerable<IHandlerFilter> filters)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(filters);

        var filterList = filters.ToArray();
        var plainFilters = filterList.Where(filter => string.IsNullOrWhiteSpace(filter.Group));
        var groupedFilters = filterList
            .Where(filter => !string.IsNullOrWhiteSpace(filter.Group))
            .GroupBy(filter => filter.Group!, StringComparer.Ordinal);

        return plainFilters.All(filter => Evaluate(filter, context))
            && groupedFilters.All(group => group.Any(filter => Evaluate(filter, context)));
    }

    public static bool Matches(IContext context, Type handlerType, MethodInfo? method = null)
    {
        ArgumentNullException.ThrowIfNull(handlerType);

        return Matches(context, GetFilters(handlerType, method));
    }

    public static IReadOnlyList<HandlerFilterAttribute> GetFilters(Type handlerType, MethodInfo? method = null)
    {
        ArgumentNullException.ThrowIfNull(handlerType);

        var filters = handlerType.GetCustomAttributes<HandlerFilterAttribute>(inherit: true);
        if (method is not null)
        {
            filters = filters.Concat(method.GetCustomAttributes<HandlerFilterAttribute>(inherit: true));
        }

        return filters.ToArray();
    }

    private static bool Evaluate(IHandlerFilter filter, IContext context)
    {
        var matched = filter.Matches(context);
        return filter.Negate ? !matched : matched;
    }
}
