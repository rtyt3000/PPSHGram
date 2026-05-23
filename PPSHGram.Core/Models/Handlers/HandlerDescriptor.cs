using System.Reflection;
using PPSHGram.Core.Models.Filters;

namespace PPSHGram.Core.Models.Handlers;

public sealed record HandlerDescriptor(
    Type HandlerType,
    MethodInfo Method,
    Type ContextType,
    HandlerKind Kind,
    IReadOnlyList<HandlerFilterAttribute> Filters);
