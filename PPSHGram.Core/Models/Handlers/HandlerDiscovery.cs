using System.Reflection;
using PPSHGram.Core.Models.Exeptions;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Models.Handlers;

public static class HandlerDiscovery
{
    private const BindingFlags HandlerMethodFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static IReadOnlyList<HandlerDescriptor> Discover(Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(handlerType);

        var handlers = new List<HandlerDescriptor>();
        var classBasedMethod = TryDiscoverClassBasedHandler(handlerType);
        if (classBasedMethod is not null)
        {
            handlers.Add(CreateDescriptor(handlerType, classBasedMethod, HandlerKind.ClassBased));
        }

        foreach (var method in DiscoverFunctionBasedMethods(handlerType, classBasedMethod))
        {
            handlers.Add(CreateDescriptor(handlerType, method, HandlerKind.FunctionBased));
        }

        return handlers;
    }

    public static IReadOnlyList<HandlerDescriptor> Discover(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .SelectMany(Discover)
            .ToArray();
    }

    private static MethodInfo? TryDiscoverClassBasedHandler(Type handlerType)
    {
        var classBasedAttribute = handlerType.GetCustomAttribute<ClassBasedHandlerAttribute>(inherit: false);
        var classHasFilters = handlerType.GetCustomAttributes<HandlerFilterAttribute>(inherit: true).Any();

        if (classBasedAttribute is null && !classHasFilters)
        {
            return null;
        }

        var methodName = classBasedAttribute?.MethodName ?? "Handle";
        var method = handlerType.GetMethod(methodName, HandlerMethodFlags);
        if (method is null && classBasedAttribute is not null)
        {
            throw new HandlerContextValidationException(
                $"Class-based handler {handlerType.Name} must declare a {methodName} method.");
        }

        return method;
    }

    private static IEnumerable<MethodInfo> DiscoverFunctionBasedMethods(Type handlerType, MethodInfo? classBasedMethod)
    {
        foreach (var method in handlerType.GetMethods(HandlerMethodFlags))
        {
            if (method.IsSpecialName || method == classBasedMethod)
            {
                continue;
            }

            var isFunctionBased = method.GetCustomAttribute<FunctionBasedHandlerAttribute>(inherit: false) is not null;
            var hasMethodFilters = method.GetCustomAttributes<HandlerFilterAttribute>(inherit: true).Any();
            if (isFunctionBased || hasMethodFilters)
            {
                yield return method;
            }
        }
    }

    private static HandlerDescriptor CreateDescriptor(Type handlerType, MethodInfo method, HandlerKind kind)
    {
        HandlerFilterEvaluator.ValidateHandlerContext(handlerType, method);
        var filters = HandlerFilterEvaluator.GetFilters(handlerType, method);
        var contextType = HandlerFilterEvaluator.ResolveContextType(filters);

        return new HandlerDescriptor(handlerType, method, contextType, kind, filters);
    }
}
