using System.Text.RegularExpressions;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.Parsing;

internal static partial class TelegramDocsParser
{
    public static TelegramSchema Parse(string html, string sourceUrl)
    {
        var blocks = ReadBlocks(html);
        var types = new List<TelegramType>();
        var methods = new List<TelegramMethod>();

        foreach (var block in blocks)
        {
            var table = ParseFirstTable(block.Html);
            if (table is null)
            {
                continue;
            }

            if (HeadersEqual(table.Headers, ["Field", "Type", "Description"]))
            {
                types.Add(new TelegramType(block.Name, block.Description, ParseFields(table.Rows)));
                continue;
            }

            if (HeadersEqual(table.Headers, ["Parameter", "Type", "Required", "Description"]))
            {
                methods.Add(new TelegramMethod(block.Name, block.Description, ParseReturnType(block.Html), ParseParameters(table.Rows)));
            }
        }

        return new TelegramSchema(
            sourceUrl,
            DateTimeOffset.UtcNow,
            ParseBotApiVersion(html),
            types.OrderBy(type => type.Name, StringComparer.Ordinal).ToArray(),
            methods.OrderBy(method => method.Name, StringComparer.Ordinal).ToArray());
    }

    private static bool HeadersEqual(IReadOnlyList<string> actual, IReadOnlyList<string> expected)
    {
        return actual.Count == expected.Count
            && actual.Zip(expected).All(pair => pair.First.Equals(pair.Second, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<DocBlock> ReadBlocks(string html)
    {
        var matches = HeadingRegex().Matches(html);
        var blocks = new List<DocBlock>(matches.Count);

        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var start = match.Index + match.Length;
            var end = i + 1 < matches.Count ? matches[i + 1].Index : html.Length;
            var body = html[start..end];
            var name = HtmlTools.ToOneLineText(match.Groups["name"].Value);

            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            blocks.Add(new DocBlock(name, ReadDescription(body), body));
        }

        return blocks;
    }

    private static string ReadDescription(string html)
    {
        var tableIndex = html.IndexOf("<table", StringComparison.OrdinalIgnoreCase);
        var descriptionHtml = tableIndex >= 0 ? html[..tableIndex] : html;
        return HtmlTools.ToOneLineText(descriptionHtml);
    }

    private static DocTable? ParseFirstTable(string html)
    {
        var tableMatch = TableRegex().Match(html);
        if (!tableMatch.Success)
        {
            return null;
        }

        var rows = new List<IReadOnlyList<string>>();
        foreach (Match rowMatch in RowRegex().Matches(tableMatch.Groups["body"].Value))
        {
            var cells = new List<string>();
            foreach (Match cellMatch in CellRegex().Matches(rowMatch.Groups["body"].Value))
            {
                cells.Add(HtmlTools.ToOneLineText(cellMatch.Groups["body"].Value));
            }

            if (cells.Count > 0)
            {
                rows.Add(cells);
            }
        }

        if (rows.Count == 0)
        {
            return null;
        }

        return new DocTable(rows[0], rows.Skip(1).ToArray());
    }

    private static IReadOnlyList<TelegramField> ParseFields(IEnumerable<IReadOnlyList<string>> rows)
    {
        return rows
            .Where(row => row.Count >= 3)
            .Select(row => new TelegramField(
                row[0],
                row[1],
                !IsOptional(row[2]),
                StripOptionalPrefix(row[2])))
            .ToArray();
    }

    private static IReadOnlyList<TelegramField> ParseParameters(IEnumerable<IReadOnlyList<string>> rows)
    {
        return rows
            .Where(row => row.Count >= 4)
            .Select(row => new TelegramField(
                row[0],
                row[1],
                row[2].Equals("Yes", StringComparison.OrdinalIgnoreCase),
                row[3]))
            .ToArray();
    }

    private static bool IsOptional(string description)
    {
        return description.StartsWith("Optional.", StringComparison.OrdinalIgnoreCase);
    }

    private static string StripOptionalPrefix(string description)
    {
        return OptionalPrefixRegex().Replace(description, string.Empty).Trim();
    }

    private static string ParseReturnType(string html)
    {
        var text = HtmlTools.ToOneLineText(html);
        var match = ReturnTypeRegex().Match(text);
        return match.Success ? match.Groups["type"].Value.Trim() : "Object";
    }

    private static string? ParseBotApiVersion(string html)
    {
        var text = HtmlTools.ToOneLineText(html);
        var match = BotApiVersionRegex().Match(text);
        return match.Success ? match.Groups["version"].Value : null;
    }

    private sealed record DocBlock(string Name, string Description, string Html);

    private sealed record DocTable(IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyList<string>> Rows);

    [GeneratedRegex(@"<h4[^>]*>(?<name>.*?)</h4>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex HeadingRegex();

    [GeneratedRegex(@"<table[^>]*>(?<body>.*?)</table>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TableRegex();

    [GeneratedRegex(@"<tr[^>]*>(?<body>.*?)</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex RowRegex();

    [GeneratedRegex(@"<t[dh][^>]*>(?<body>.*?)</t[dh]>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex CellRegex();

    [GeneratedRegex(@"^Optional\.\s*", RegexOptions.IgnoreCase)]
    private static partial Regex OptionalPrefixRegex();

    [GeneratedRegex(@"Returns\s+(?:an?\s+)?(?<type>.+?)(?:\.| on success|$)", RegexOptions.IgnoreCase)]
    private static partial Regex ReturnTypeRegex();

    [GeneratedRegex(@"Bot API (?<version>\d+(?:\.\d+)*)", RegexOptions.IgnoreCase)]
    private static partial Regex BotApiVersionRegex();
}
