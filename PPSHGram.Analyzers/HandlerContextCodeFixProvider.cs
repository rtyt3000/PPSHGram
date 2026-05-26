using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace PPSHGram.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HandlerContextCodeFixProvider))]
[Shared]
public sealed class HandlerContextCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(
            HandlerContextAnalyzer.ContextMismatchDiagnosticId,
            HandlerContextAnalyzer.MissingContextDiagnosticId);

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics[0];
        if (!diagnostic.Properties.TryGetValue(
                HandlerContextAnalyzer.RequiredContextMetadataNameProperty,
                out var requiredContext)
            || string.IsNullOrWhiteSpace(requiredContext))
        {
            return;
        }

        var displayName = diagnostic.Properties.TryGetValue(
                HandlerContextAnalyzer.RequiredContextDisplayNameProperty,
                out var value)
            && !string.IsNullOrWhiteSpace(value)
                ? value!
                : requiredContext!;

        var root = await context.Document
            .GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                $"Use {displayName} context parameter",
                cancellationToken => FixContextParameterAsync(context.Document, root, diagnostic, requiredContext!, cancellationToken),
                equivalenceKey: "UseRequiredContextParameter"),
            diagnostic);
    }

    private static Task<Document> FixContextParameterAsync(
        Document document,
        SyntaxNode root,
        Diagnostic diagnostic,
        string requiredContext,
        CancellationToken cancellationToken)
    {
        var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
        var requiredContextType = SyntaxFactory.ParseTypeName(requiredContext)
            .WithAdditionalAnnotations(Simplifier.Annotation);

        if (node.FirstAncestorOrSelf<ParameterSyntax>() is { } parameter)
        {
            var fixedParameter = parameter.WithType(requiredContextType.WithTriviaFrom(parameter.Type ?? requiredContextType));
            var fixedRoot = root.ReplaceNode(parameter, fixedParameter);
            return Task.FromResult(document.WithSyntaxRoot(fixedRoot));
        }

        if (node.FirstAncestorOrSelf<MethodDeclarationSyntax>() is { } method)
        {
            var contextParameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("context"))
                .WithType(requiredContextType);
            var parameters = method.ParameterList.Parameters.Insert(0, contextParameter);
            var fixedMethod = method.WithParameterList(method.ParameterList.WithParameters(parameters));
            var fixedRoot = root.ReplaceNode(method, fixedMethod);
            return Task.FromResult(document.WithSyntaxRoot(fixedRoot));
        }

        return Task.FromResult(document);
    }
}
