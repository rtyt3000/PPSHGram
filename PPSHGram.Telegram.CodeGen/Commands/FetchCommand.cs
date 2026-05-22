using PPSHGram.Telegram.CodeGen.Cli;

namespace PPSHGram.Telegram.CodeGen.Commands;

internal sealed class FetchCommand(AppPaths paths, CliOptions options) : ICommand
{
    public const string DefaultUrl = "https://core.telegram.org/bots/api";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var url = options.Get("url") ?? DefaultUrl;
        var outputPath = paths.ResolveHtml(options);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? paths.RootDirectory);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PPSHGram.Telegram.CodeGen/1.0");

        var html = await httpClient.GetStringAsync(url, cancellationToken);
        await File.WriteAllTextAsync(outputPath, html, cancellationToken);

        Console.WriteLine($"Fetched {url}");
        Console.WriteLine($"Wrote {Path.GetRelativePath(paths.RootDirectory, outputPath)}");
    }
}
