using System.Globalization;
using System.Text;

namespace PPSHGram.Telegram.CodeGen.CSharp;

internal static class CSharpNaming
{
    private static readonly HashSet<string> Keywords =
    [
        "base",
        "class",
        "event",
        "namespace",
        "object",
        "operator",
        "params",
        "string"
    ];

    public static string ToPascalCase(string value)
    {
        var builder = new StringBuilder(value.Length);
        var upperNext = true;

        foreach (var character in value)
        {
            if (!char.IsLetterOrDigit(character))
            {
                upperNext = true;
                continue;
            }

            builder.Append(upperNext ? char.ToUpper(character, CultureInfo.InvariantCulture) : character);
            upperNext = false;
        }

        var result = builder.Length == 0 ? "Value" : builder.ToString();
        if (char.IsDigit(result[0]))
        {
            result = $"_{result}";
        }

        return Keywords.Contains(result) ? $"@{result}" : result;
    }

    public static string ToPropertyName(string value, string containingTypeName)
    {
        var propertyName = ToPascalCase(value);
        var comparablePropertyName = propertyName.TrimStart('@');

        return comparablePropertyName.Equals(containingTypeName, StringComparison.Ordinal)
            ? $"{propertyName}Value"
            : propertyName;
    }
}
