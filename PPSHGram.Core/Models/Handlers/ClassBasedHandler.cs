namespace PPSHGram.Core.Models.Handlers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ClassBasedHandlerAttribute : Attribute
{
    public string MethodName { get; init; } = "Handle";
}
