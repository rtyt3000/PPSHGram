using System.Text.RegularExpressions;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static partial class CSharpTemplateRenderer
{
    private static readonly Lazy<string> TemplatesDirectory = new(FindTemplatesDirectory);

    public static string Render(string templateName, IReadOnlyDictionary<string, string> values)
    {
        var templatePath = Path.Combine(TemplatesDirectory.Value, templateName);
        var text = File.ReadAllText(templatePath);

        foreach (var (key, value) in values)
        {
            text = text.Replace($"{{{{{key}}}}}", value, StringComparison.Ordinal);
        }

        var missingPlaceholder = PlaceholderRegex().Match(text);
        if (missingPlaceholder.Success)
        {
            throw new InvalidOperationException(
                $"Template '{templateName}' contains an unresolved placeholder: {missingPlaceholder.Value}");
        }

        return NormalizeLineEndings(text);
    }

    private static string NormalizeLineEndings(string text)
    {
        var normalized = text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);

        return normalized.Replace("\n", Environment.NewLine, StringComparison.Ordinal);
    }

    private static string FindTemplatesDirectory()
    {
        var outputTemplatesDirectory = Path.Combine(AppContext.BaseDirectory, "Templates");
        if (Directory.Exists(outputTemplatesDirectory))
        {
            return outputTemplatesDirectory;
        }

        var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        while (currentDirectory is not null)
        {
            var projectTemplatesDirectory = Path.Combine(
                currentDirectory.FullName,
                "PPSHGram.Telegram.CodeGen",
                "Templates");
            if (Directory.Exists(projectTemplatesDirectory))
            {
                return projectTemplatesDirectory;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find PPSHGram.Telegram.CodeGen Templates directory.");
    }

    [GeneratedRegex(@"\{\{[A-Za-z0-9_]+\}\}")]
    private static partial Regex PlaceholderRegex();
}
