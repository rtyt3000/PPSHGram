using System.Text.RegularExpressions;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static partial class CSharpTypeMapper
{
    public static string Map(string telegramType, bool required)
    {
        var normalized = Normalize(telegramType);
        var result = MapRequired(normalized);

        if (!required && CanBeNullable(result))
        {
            result += "?";
        }

        return result;
    }

    public static bool NeedsDefaultInitializer(string csharpType, bool required)
    {
        return required
            && !csharpType.EndsWith("?", StringComparison.Ordinal)
            && !IsKnownValueType(csharpType);
    }

    public static IReadOnlyList<string> GetReferencedTypeNames(string telegramType)
    {
        return SplitUnionParts(Normalize(telegramType))
            .Select(UnwrapArray)
            .Select(MapRequired)
            .SelectMany(ExtractGenericArguments)
            .Where(IsPotentialTypeName)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static string MapRequired(string type)
    {
        if (TryMapArray(type, out var arrayType))
        {
            return arrayType;
        }

        if (type.Contains(" or ", StringComparison.OrdinalIgnoreCase))
        {
            return "object";
        }

        return type switch
        {
            "String" => "string",
            "Integer" => "long",
            "Int" => "long",
            "Float" => "double",
            "Double" => "double",
            "Number" => "double",
            "Boolean" => "bool",
            "Bool" => "bool",
            "True" => "bool",
            "False" => "bool",
            "Object" => "object",
            _ => CSharpNaming.ToPascalCase(type)
        };
    }

    private static bool TryMapArray(string type, out string csharpType)
    {
        var match = ArrayRegex().Match(type);
        if (!match.Success)
        {
            csharpType = string.Empty;
            return false;
        }

        var inner = MapRequired(match.Groups["inner"].Value);
        csharpType = $"IReadOnlyList<{inner}>";
        return true;
    }

    private static string Normalize(string type)
    {
        var normalized = type.Replace(",", " or ", StringComparison.Ordinal);
        normalized = AndRegex().Replace(normalized, " or ");
        normalized = WhitespaceRegex().Replace(normalized, " ").Trim();
        return normalized;
    }

    private static IEnumerable<string> SplitUnionParts(string type)
    {
        return OrRegex()
            .Split(type)
            .Select(part => part.Trim())
            .Where(part => part.Length > 0);
    }

    private static string UnwrapArray(string type)
    {
        while (true)
        {
            var match = ArrayRegex().Match(type);
            if (!match.Success)
            {
                return type;
            }

            type = match.Groups["inner"].Value;
        }
    }

    private static IEnumerable<string> ExtractGenericArguments(string csharpType)
    {
        if (!csharpType.StartsWith("IReadOnlyList<", StringComparison.Ordinal))
        {
            yield return csharpType;
            yield break;
        }

        var inner = csharpType["IReadOnlyList<".Length..^1];
        foreach (var typeName in ExtractGenericArguments(inner))
        {
            yield return typeName;
        }
    }

    private static bool IsPotentialTypeName(string csharpType)
    {
        return csharpType is not "string"
            and not "long"
            and not "int"
            and not "double"
            and not "bool"
            and not "object"
            && csharpType.Length > 0
            && char.IsUpper(csharpType[0]);
    }

    private static bool CanBeNullable(string csharpType)
    {
        return !csharpType.EndsWith("?", StringComparison.Ordinal);
    }

    private static bool IsKnownValueType(string csharpType)
    {
        return csharpType is "long" or "int" or "double" or "bool";
    }

    [GeneratedRegex(@"^Array of (?<inner>.+)$", RegexOptions.IgnoreCase)]
    private static partial Regex ArrayRegex();

    [GeneratedRegex(@"\s+or\s+", RegexOptions.IgnoreCase)]
    private static partial Regex OrRegex();

    [GeneratedRegex(@"\s+and\s+", RegexOptions.IgnoreCase)]
    private static partial Regex AndRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
