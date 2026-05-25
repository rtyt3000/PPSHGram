using System.Text.RegularExpressions;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static partial class CSharpTypeMapper
{
    public static string Map(TelegramField field)
    {
        return Map(field.Type, field.Required, field.Name);
    }

    public static string Map(string telegramType, bool required, string? fieldName = null)
    {
        var normalized = Normalize(telegramType);
        var result = MapRequired(normalized, fieldName);

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

    public static IReadOnlyList<string> GetReferencedTypeNames(TelegramField field)
    {
        return GetReferencedTypeNames(field.Type, field.Name);
    }

    public static IReadOnlyList<string> GetReferencedTypeNames(string telegramType, string? fieldName = null)
    {
        var normalized = Normalize(telegramType);
        var referencedTypes = SplitUnionParts(normalized)
            .Select(UnwrapArray)
            .Select(part => MapRequired(part, null))
            .SelectMany(ExtractGenericArguments)
            .Where(IsPotentialTypeName)
            .ToArray();

        if (!TryGetUnionMapping(normalized, fieldName, out var unionMapping))
        {
            return referencedTypes
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }

        return referencedTypes
            .Append(unionMapping.TypeName)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    public static CSharpUnionMapping? GetUnionMapping(TelegramField field)
    {
        return TryGetUnionMapping(Normalize(field.Type), field.Name, out var unionMapping)
            ? unionMapping
            : null;
    }

    private static string MapRequired(string type, string? fieldName)
    {
        if (TryMapArray(type, fieldName, out var arrayType))
        {
            return arrayType;
        }

        if (TryGetUnionMapping(type, fieldName, out var unionMapping))
        {
            return unionMapping.TypeName;
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

    private static bool TryMapArray(string type, string? fieldName, out string csharpType)
    {
        var match = ArrayRegex().Match(type);
        if (!match.Success)
        {
            csharpType = string.Empty;
            return false;
        }

        var inner = MapRequired(match.Groups["inner"].Value, fieldName);
        csharpType = $"IReadOnlyList<{inner}>";
        return true;
    }

    private static bool TryGetUnionMapping(
        string type,
        string? fieldName,
        out CSharpUnionMapping unionMapping)
    {
        var arrayMatch = ArrayRegex().Match(type);
        if (arrayMatch.Success)
        {
            return TryGetUnionMapping(arrayMatch.Groups["inner"].Value, fieldName, out unionMapping);
        }

        if (!type.Contains(" or ", StringComparison.OrdinalIgnoreCase))
        {
            unionMapping = default!;
            return false;
        }

        var partTypeNames = SplitUnionParts(type)
            .Select(part => MapRequired(part, null))
            .ToArray();

        if (partTypeNames.Length < 2 || partTypeNames.Any(part => !IsPotentialTypeName(part)))
        {
            unionMapping = default!;
            return false;
        }

        var unionTypeName = InferUnionTypeName(partTypeNames, fieldName);
        if (unionTypeName.Length == 0)
        {
            unionMapping = default!;
            return false;
        }

        unionMapping = new CSharpUnionMapping(unionTypeName, partTypeNames);
        return true;
    }

    private static string InferUnionTypeName(IReadOnlyList<string> partTypeNames, string? fieldName)
    {
        var commonPrefix = GetCommonTypePrefix(partTypeNames);
        if (commonPrefix.Length > 0)
        {
            return commonPrefix;
        }

        return string.IsNullOrWhiteSpace(fieldName)
            ? string.Empty
            : CSharpNaming.ToPascalCase(fieldName);
    }

    private static string GetCommonTypePrefix(IReadOnlyList<string> typeNames)
    {
        var prefix = typeNames[0];
        foreach (var typeName in typeNames.Skip(1))
        {
            var length = 0;
            while (length < prefix.Length
                && length < typeName.Length
                && prefix[length] == typeName[length])
            {
                length++;
            }

            prefix = prefix[..length];
        }

        while (prefix.Length > 0 && typeNames.Any(typeName => !IsWordBoundary(typeName, prefix.Length)))
        {
            prefix = prefix[..^1];
        }

        return prefix;
    }

    private static bool IsWordBoundary(string typeName, int index)
    {
        return index == typeName.Length || char.IsUpper(typeName[index]) || char.IsDigit(typeName[index]);
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

internal sealed record CSharpUnionMapping(string TypeName, IReadOnlyList<string> PartTypeNames);
