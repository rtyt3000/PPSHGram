using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static partial class CSharpXmlDocs
{
    public static string WriteSummary(string text, int indent = 0)
    {
        var builder = new StringBuilder();
        WriteSummary(builder, text, indent);
        return builder.ToString();
    }

    public static void WriteSummary(StringBuilder builder, string text, int indent = 0)
    {
        var normalized = Normalize(text);
        if (normalized.Length == 0)
        {
            return;
        }

        var indentation = new string(' ', indent);
        builder.AppendLine($"{indentation}/// <summary>");
        builder.AppendLine($"{indentation}/// {SecurityElement.Escape(normalized)}");
        builder.AppendLine($"{indentation}/// </summary>");
    }

    public static void WriteParameterRemarks(
        StringBuilder builder,
        IEnumerable<(string Name, string Type, bool Required, string Description)> parameters,
        int indent = 0)
    {
        var parameterList = parameters.ToArray();
        if (parameterList.Length == 0)
        {
            return;
        }

        var indentation = new string(' ', indent);
        builder.AppendLine($"{indentation}/// <remarks>");
        builder.AppendLine($"{indentation}/// <para>Parameters:</para>");
        builder.AppendLine($"{indentation}/// <list type=\"bullet\">");

        foreach (var parameter in parameterList)
        {
            var requirement = parameter.Required ? "Required" : "Optional";
            var description = Normalize(parameter.Description);
            var type = Normalize(parameter.Type);

            builder.AppendLine($"{indentation}/// <item>");
            builder.AppendLine($"{indentation}/// <term>{SecurityElement.Escape(parameter.Name)}</term>");
            builder.AppendLine($"{indentation}/// <description>{SecurityElement.Escape(type)}. {requirement}. {SecurityElement.Escape(description)}</description>");
            builder.AppendLine($"{indentation}/// </item>");
        }

        builder.AppendLine($"{indentation}/// </list>");
        builder.AppendLine($"{indentation}/// </remarks>");
    }

    public static string WriteParameterRemarks(
        IEnumerable<(string Name, string Type, bool Required, string Description)> parameters,
        int indent = 0)
    {
        var builder = new StringBuilder();
        WriteParameterRemarks(builder, parameters, indent);
        return builder.ToString();
    }

    private static string Normalize(string text)
    {
        return WhitespaceRegex().Replace(text, " ").Trim();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
