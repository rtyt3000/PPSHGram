using System.Net;
using System.Text.RegularExpressions;

namespace PPSHGram.Telegram.CodeGen.Parsing;

internal static partial class HtmlTools
{
    public static string ToText(string html)
    {
        var text = BreakTagsRegex().Replace(html, "\n");
        text = TagsRegex().Replace(text, string.Empty);
        text = WebUtility.HtmlDecode(text);
        text = WhitespaceRegex().Replace(text, " ");
        return text.Trim();
    }

    public static string ToOneLineText(string html)
    {
        return SingleLineWhitespaceRegex().Replace(ToText(html), " ").Trim();
    }

    [GeneratedRegex(@"<\s*/\s*(?:p|div|li|tr|table|blockquote)\s*>|<\s*br\s*/?\s*>", RegexOptions.IgnoreCase)]
    private static partial Regex BreakTagsRegex();

    [GeneratedRegex("<.*?>", RegexOptions.Singleline)]
    private static partial Regex TagsRegex();

    [GeneratedRegex(@"[ \t\r\n]+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex SingleLineWhitespaceRegex();
}
