using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PPSHGram.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HandlerContextAnalyzer : DiagnosticAnalyzer
{
    internal const string ContextMismatchDiagnosticId = "PPSHG001";
    internal const string MissingContextDiagnosticId = "PPSHG002";
    internal const string RequiredContextMetadataNameProperty = "RequiredContextMetadataName";
    internal const string RequiredContextDisplayNameProperty = "RequiredContextDisplayName";

    private const string HandlerFilterAttributeName = "PPSHGram.Core.Models.Filters.HandlerFilterAttribute";
    private const string RequiresContextAttributeName = "PPSHGram.Core.Models.Filters.RequiresContextAttribute";
    private const string ClassBasedHandlerAttributeName = "PPSHGram.Core.Models.Handlers.ClassBasedHandlerAttribute";
    private const string FunctionBasedHandlerAttributeName = "PPSHGram.Core.Models.Handlers.FunctionBasedHandlerAttribute";
    // ReSharper disable once InconsistentNaming
    private const string IContextName = "PPSHGram.Core.Models.Context.IContext";

    private static readonly DiagnosticDescriptor ContextMismatchRule = new(
        ContextMismatchDiagnosticId,
        "Handler context does not match filters",
        "Handler '{0}' accepts '{1}', but filters require '{2}'",
        "PPSHGram.Handlers",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingContextRule = new(
        MissingContextDiagnosticId,
        "Handler is missing context parameter",
        "Handler '{0}' must accept '{1}' as its first parameter",
        "PPSHGram.Handlers",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor IncompatibleFiltersRule = new(
        "PPSHG003",
        "Handler filters require incompatible contexts",
        "Handler '{0}' has filters that require incompatible contexts '{1}' and '{2}'",
        "PPSHGram.Handlers",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [ContextMismatchRule, MissingContextRule, IncompatibleFiltersRule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(startContext =>
        {
            var symbols = AnalyzerSymbols.Create(startContext.Compilation);
            if (symbols is null) return;

            startContext.RegisterSymbolAction(
                symbolContext => AnalyzeType((INamedTypeSymbol)symbolContext.Symbol, symbols, symbolContext),
                SymbolKind.NamedType);

            startContext.RegisterSymbolAction(
                symbolContext => AnalyzeMethod((IMethodSymbol)symbolContext.Symbol, symbols, symbolContext),
                SymbolKind.Method);
        });
    }

    private static void AnalyzeType(INamedTypeSymbol type, AnalyzerSymbols symbols, SymbolAnalysisContext context)
    {
        if (type.TypeKind != TypeKind.Class || type.IsAbstract)
        {
            return;
        }

        var classFilters = GetFilterAttributes(type.GetAttributes(), symbols).ToArray();
        var classBasedAttribute = GetAttribute(type.GetAttributes(), symbols.ClassBasedHandlerAttribute);
        if (classFilters.Length == 0 && classBasedAttribute is null)
        {
            return;
        }

        var methodName = GetClassBasedMethodName(classBasedAttribute);
        var method = type.GetMembers(methodName).OfType<IMethodSymbol>().FirstOrDefault(method => !method.IsStatic);
        if (method is null)
        {
            return;
        }

        var filters = classFilters
            .Concat(GetFilterAttributes(method.GetAttributes(), symbols))
            .ToArray();

        ValidateMethod(type, method, filters, symbols, context);
    }

    private static void AnalyzeMethod(IMethodSymbol method, AnalyzerSymbols symbols, SymbolAnalysisContext context)
    {
        if (method.MethodKind != MethodKind.Ordinary || method.IsStatic || method.AssociatedSymbol is not null)
        {
            return;
        }

        var methodAttributes = method.GetAttributes();
        var methodFilters = GetFilterAttributes(methodAttributes, symbols).ToArray();
        var functionBasedAttribute = GetAttribute(methodAttributes, symbols.FunctionBasedHandlerAttribute);
        if (methodFilters.Length == 0 && functionBasedAttribute is null)
        {
            return;
        }

        if (method.ContainingType is null)
        {
            return;
        }

        if (IsDiscoveredAsClassBasedMethod(method, symbols))
        {
            return;
        }

        var filters = GetFilterAttributes(method.ContainingType.GetAttributes(), symbols)
            .Concat(methodFilters)
            .ToArray();

        ValidateMethod(method.ContainingType, method, filters, symbols, context);
    }

    private static void ValidateMethod(
        INamedTypeSymbol handlerType,
        IMethodSymbol method,
        IReadOnlyList<AttributeData> filters,
        AnalyzerSymbols symbols,
        SymbolAnalysisContext context)
    {
        if (!TryResolveRequiredContext(filters, symbols, context.Compilation, out var requiredContext, out var conflict))
        {
            if (conflict is not null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    IncompatibleFiltersRule,
                    method.Locations.FirstOrDefault(),
                    $"{handlerType.Name}.{method.Name}",
                    conflict.Value.Left.Name,
                    conflict.Value.Right.Name));
            }

            return;
        }

        requiredContext ??= symbols.IContext;
        var parameter = method.Parameters.FirstOrDefault();
        if (parameter is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                MissingContextRule,
                method.Locations.FirstOrDefault(),
                CreateRequiredContextProperties(requiredContext),
                $"{handlerType.Name}.{method.Name}",
                requiredContext.Name));
            return;
        }

        if (!CanAssign(context.Compilation, requiredContext, parameter.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                ContextMismatchRule,
                parameter.Locations.FirstOrDefault(),
                CreateRequiredContextProperties(requiredContext),
                $"{handlerType.Name}.{method.Name}",
                parameter.Type.Name,
                requiredContext.Name));
        }
    }

    private static ImmutableDictionary<string, string?> CreateRequiredContextProperties(ITypeSymbol requiredContext)
    {
        return ImmutableDictionary<string, string?>.Empty
            .Add(RequiredContextMetadataNameProperty, requiredContext.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Add(RequiredContextDisplayNameProperty, requiredContext.Name);
    }

    private static bool TryResolveRequiredContext(
        IEnumerable<AttributeData> filters,
        AnalyzerSymbols symbols,
        Compilation compilation,
        out ITypeSymbol? requiredContext,
        out (ITypeSymbol Left, ITypeSymbol Right)? conflict)
    {
        requiredContext = symbols.IContext;
        conflict = null;

        foreach (var filter in filters)
        {
            var candidate = GetRequiredContext(filter, symbols);
            if (candidate is null)
            {
                continue;
            }

            if (CanAssign(compilation, candidate, requiredContext))
            {
                requiredContext = candidate;
                continue;
            }

            if (CanAssign(compilation, requiredContext, candidate))
            {
                continue;
            }

            conflict = (requiredContext, candidate);
            return false;
        }

        return true;
    }

    private static ITypeSymbol? GetRequiredContext(AttributeData filter, AnalyzerSymbols symbols)
    {
        var filterType = filter.AttributeClass;
        if (filterType is null)
        {
            return null;
        }

        foreach (var attribute in filterType.GetAttributes())
        {
            if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, symbols.RequiresContextAttribute))
            {
                continue;
            }

            return attribute.ConstructorArguments.FirstOrDefault().Value as ITypeSymbol;
        }

        return null;
    }

    private static IEnumerable<AttributeData> GetFilterAttributes(
        ImmutableArray<AttributeData> attributes,
        AnalyzerSymbols symbols)
    {
        return attributes.Where(attribute => IsFilterAttribute(attribute.AttributeClass, symbols.HandlerFilterAttribute));
    }

    private static AttributeData? GetAttribute(ImmutableArray<AttributeData> attributes, INamedTypeSymbol attributeType)
    {
        return attributes.FirstOrDefault(attribute =>
            SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType));
    }

    private static bool IsFilterAttribute(INamedTypeSymbol? type, INamedTypeSymbol handlerFilterAttribute)
    {
        while (type is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(type, handlerFilterAttribute))
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static string GetClassBasedMethodName(AttributeData? classBasedAttribute)
    {
        if (classBasedAttribute is null)
        {
            return "Handle";
        }

        foreach (var argument in classBasedAttribute.NamedArguments)
        {
            if (argument is { Key: "MethodName", Value.Value: string methodName })
            {
                return methodName;
            }
        }

        return "Handle";
    }

    private static bool IsDiscoveredAsClassBasedMethod(IMethodSymbol method, AnalyzerSymbols symbols)
    {
        var containingType = method.ContainingType;
        if (containingType is null)
        {
            return false;
        }

        var typeAttributes = containingType.GetAttributes();
        var classHasFilters = GetFilterAttributes(typeAttributes, symbols).Any();
        var classBasedAttribute = GetAttribute(typeAttributes, symbols.ClassBasedHandlerAttribute);

        if (!classHasFilters && classBasedAttribute is null)
        {
            return false;
        }

        return method.Name == GetClassBasedMethodName(classBasedAttribute);
    }

    private static bool CanAssign(Compilation compilation, ITypeSymbol from, ITypeSymbol to)
    {
        return compilation.ClassifyCommonConversion(from, to).IsImplicit;
    }

    private sealed class AnalyzerSymbols
    {
        private AnalyzerSymbols(
            INamedTypeSymbol handlerFilterAttribute,
            INamedTypeSymbol requiresContextAttribute,
            INamedTypeSymbol classBasedHandlerAttribute,
            INamedTypeSymbol functionBasedHandlerAttribute,
            INamedTypeSymbol iContext)
        {
            HandlerFilterAttribute = handlerFilterAttribute;
            RequiresContextAttribute = requiresContextAttribute;
            ClassBasedHandlerAttribute = classBasedHandlerAttribute;
            FunctionBasedHandlerAttribute = functionBasedHandlerAttribute;
            IContext = iContext;
        }

        public INamedTypeSymbol HandlerFilterAttribute { get; }

        public INamedTypeSymbol RequiresContextAttribute { get; }

        public INamedTypeSymbol ClassBasedHandlerAttribute { get; }

        public INamedTypeSymbol FunctionBasedHandlerAttribute { get; }

        public INamedTypeSymbol IContext { get; }

        public static AnalyzerSymbols? Create(Compilation compilation)
        {
            var handlerFilterAttribute = compilation.GetTypeByMetadataName(HandlerFilterAttributeName);
            var requiresContextAttribute = compilation.GetTypeByMetadataName(RequiresContextAttributeName);
            var classBasedHandlerAttribute = compilation.GetTypeByMetadataName(ClassBasedHandlerAttributeName);
            var functionBasedHandlerAttribute = compilation.GetTypeByMetadataName(FunctionBasedHandlerAttributeName);
            var iContext = compilation.GetTypeByMetadataName(IContextName);

            if (handlerFilterAttribute is null
                || requiresContextAttribute is null
                || classBasedHandlerAttribute is null
                || functionBasedHandlerAttribute is null
                || iContext is null)
            {
                return null;
            }

            return new AnalyzerSymbols(
                handlerFilterAttribute,
                requiresContextAttribute,
                classBasedHandlerAttribute,
                functionBasedHandlerAttribute,
                iContext);
        }
    }
}
