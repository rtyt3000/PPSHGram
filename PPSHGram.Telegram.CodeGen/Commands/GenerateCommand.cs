using System.Text.Json;
using PPSHGram.Telegram.CodeGen.Cli;
using PPSHGram.Telegram.CodeGen.CSharp;
using PPSHGram.Telegram.CodeGen.Schema;

namespace PPSHGram.Telegram.CodeGen.Commands;

internal sealed class GenerateCommand(AppPaths paths, CliOptions options) : ICommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var schemaPath = paths.ResolveSchema(options);
        var outputPath = paths.ResolveOutput(options);

        if (!File.Exists(schemaPath))
        {
            throw new CliException($"Schema JSON was not found: {schemaPath}");
        }

        var json = await File.ReadAllTextAsync(schemaPath, cancellationToken);
        var schema = JsonSerializer.Deserialize(json, SchemaJsonContext.Default.TelegramSchema)
            ?? throw new CliException($"Could not deserialize schema JSON: {schemaPath}");

        CSharpGenerator.Generate(schema, outputPath);

        Console.WriteLine($"Generated C# into {Path.GetRelativePath(paths.RootDirectory, outputPath)}");
    }
}
