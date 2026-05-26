using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static class CSharpTypeWriter
{
    public static string WriteType(TelegramType type, IReadOnlyCollection<string>? implementedInterfaces = null)
    {
        var builder = new StringBuilder();
        foreach (var field in type.Fields)
        {
            WriteProperty(builder, type.Name, field);
        }

        return CSharpTemplateRenderer.Render(
            "Type.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary(type.Description),
                ["type_name"] = type.Name,
                ["implemented_interfaces"] = FormatImplementedInterfaces(implementedInterfaces),
                ["properties"] = builder.ToString()
            });
    }

    public static string WritePlaceholderType(
        string typeName,
        bool asBaseType = false,
        IReadOnlyCollection<string>? derivedTypeNames = null)
    {
        var derivedTypes = string.Join(
            string.Empty,
            (derivedTypeNames?.Order(StringComparer.Ordinal) ?? Enumerable.Empty<string>())
            .Select(derivedTypeName => $"[JsonDerivedType(typeof({derivedTypeName}))]{Environment.NewLine}"));
        var typeKeyword = asBaseType ? "abstract partial class" : "partial class";

        return CSharpTemplateRenderer.Render(
            "PlaceholderType.cs.template",
            new Dictionary<string, string>
            {
                ["usings"] = derivedTypeNames is { Count: > 0 }
                    ? $"using System.Text.Json.Serialization;{Environment.NewLine}"
                    : string.Empty,
                ["derived_types"] = derivedTypes,
                ["summary"] = CSharpXmlDocs.WriteSummary($"Represents the Telegram Bot API {typeName} type."),
                ["type_kind"] = typeKeyword,
                ["type_name"] = typeName
            });
    }

    private static void WriteProperty(StringBuilder builder, string containingTypeName, TelegramField field)
    {
        var csharpType = MapPropertyType(field);
        var propertyName = CSharpNaming.ToPropertyName(field.Name, containingTypeName);
        var initializer = GetPropertyInitializer(field, csharpType);

        builder.Append(CSharpTemplateRenderer.Render(
            "Property.cs.template",
            new Dictionary<string, string>
            {
                ["summary"] = CSharpXmlDocs.WriteSummary(field.Description, 4),
                ["json_name"] = field.Name,
                ["property_type"] = csharpType,
                ["property_name"] = propertyName,
                ["initializer"] = initializer
            }));
    }

    private static string MapPropertyType(TelegramField field)
    {
        if (IsAttachableStringFileField(field))
        {
            return field.Required ? "InputFile" : "InputFile?";
        }

        return CSharpTypeMapper.Map(field);
    }

    private static string GetPropertyInitializer(TelegramField field, string csharpType)
    {
        var fixedStringValue = GetFixedStringValue(field);
        if (fixedStringValue is not null)
        {
            return $" = {JsonSerializer.Serialize(fixedStringValue)};";
        }

        return CSharpTypeMapper.NeedsDefaultInitializer(csharpType, field.Required)
            ? " = default!;"
            : string.Empty;
    }

    private static string? GetFixedStringValue(TelegramField field)
    {
        if (!field.Required
            || !field.Name.Equals("type", StringComparison.Ordinal)
            || !field.Type.Equals("String", StringComparison.Ordinal))
        {
            return null;
        }

        var match = MustBeRegex.Match(field.Description);
        return match.Success ? match.Groups["value"].Value : null;
    }

    private static bool IsAttachableStringFileField(TelegramField field)
    {
        return field.Type.Equals("String", StringComparison.Ordinal)
            && field.Description.Contains("attach://", StringComparison.OrdinalIgnoreCase)
            && field.Description.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatImplementedInterfaces(IReadOnlyCollection<string>? implementedInterfaces)
    {
        if (implementedInterfaces is null || implementedInterfaces.Count == 0)
        {
            return string.Empty;
        }

        return $" : {string.Join(", ", implementedInterfaces.Order(StringComparer.Ordinal))}";
    }

    private static readonly Regex MustBeRegex = new(
        @"\bmust be (?<value>[a-z0-9_]+)\b",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
}
