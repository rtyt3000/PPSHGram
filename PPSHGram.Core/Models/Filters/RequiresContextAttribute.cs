using PPSHGram.Core.Models.Context;

namespace PPSHGram.Core.Models.Filters;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RequiresContextAttribute(Type contextType) : Attribute
{
    public Type ContextType { get; } = typeof(IContext).IsAssignableFrom(contextType)
        ? contextType
        : throw new ArgumentException("Required context type must implement IContext.", nameof(contextType));
}
