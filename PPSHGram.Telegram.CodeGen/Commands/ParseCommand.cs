using System.Text.Json;
using PPSHGram.Telegram.CodeGen.Cli;
using PPSHGram.Telegram.CodeGen.Parsing;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.Commands;

internal sealed class ParseCommand(AppPaths paths, CliOptions options) : ICommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var inputPath = paths.ResolveHtml(options);
        var outputPath = paths.ResolveSchema(options);

        if (!File.Exists(inputPath))
        {
            throw new CliException($"Input HTML was not found: {inputPath}");
        }

        var html = await File.ReadAllTextAsync(inputPath, cancellationToken);
        var schema = TelegramDocsParser.Parse(html, FetchCommand.DefaultUrl);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? paths.RootDirectory);

        var json = JsonSerializer.Serialize(schema, SchemaJsonContext.Default.TelegramSchema);
        await File.WriteAllTextAsync(outputPath, json, cancellationToken);

        Console.WriteLine($"Parsed {schema.Types.Count} types and {schema.Methods.Count} methods.");
        Console.WriteLine($"Wrote {Path.GetRelativePath(paths.RootDirectory, outputPath)}");
    }
}
