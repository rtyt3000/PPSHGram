using System.Reflection;
using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Exeptions;

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

    public static Type ResolveContextType(IEnumerable<IHandlerFilter> filters)
    {
        ArgumentNullException.ThrowIfNull(filters);

        var result = typeof(IContext);
        foreach (var filter in filters)
        {
            var candidate = filter.ContextType;
            if (!typeof(IContext).IsAssignableFrom(candidate))
            {
                throw new HandlerContextValidationException(
                    $"Filter {filter.GetType().Name} requires {candidate.Name}, but filter context types must implement {nameof(IContext)}.");
            }

            if (result.IsAssignableFrom(candidate))
            {
                result = candidate;
                continue;
            }

            if (candidate.IsAssignableFrom(result))
            {
                continue;
            }

            throw new HandlerContextValidationException(
                $"Filters require incompatible context types: {result.Name} and {candidate.Name}.");
        }

        return result;
    }

    public static Type ResolveContextType(Type handlerType, MethodInfo? method = null)
    {
        return ResolveContextType(GetFilters(handlerType, method));
    }

    public static void ValidateHandlerContext(Type handlerType, MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(handlerType);
        ArgumentNullException.ThrowIfNull(method);

        var requiredContextType = ResolveContextType(handlerType, method);
        var parameter = method.GetParameters().FirstOrDefault();
        if (parameter is null)
        {
            throw new HandlerContextValidationException(
                $"Handler {handlerType.Name}.{method.Name} must accept {requiredContextType.Name} as its first parameter.");
        }

        if (!typeof(IContext).IsAssignableFrom(parameter.ParameterType))
        {
            throw new HandlerContextValidationException(
                $"Handler {handlerType.Name}.{method.Name} first parameter must implement {nameof(IContext)}.");
        }

        if (!parameter.ParameterType.IsAssignableFrom(requiredContextType))
        {
            throw new HandlerContextValidationException(
                $"Handler {handlerType.Name}.{method.Name} accepts {parameter.ParameterType.Name}, but filters require {requiredContextType.Name}.");
        }
    }

    private static bool Evaluate(IHandlerFilter filter, IContext context)
    {
        var matched = filter.Matches(context);
        return filter.Negate ? !matched : matched;
    }
}
