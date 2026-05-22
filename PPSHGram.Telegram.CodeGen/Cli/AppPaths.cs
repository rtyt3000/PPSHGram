namespace PPSHGram.Telegram.CodeGen.Cli;

internal sealed class AppPaths
{
    private const string SolutionFile = "PPSHGram.slnx";

    private AppPaths(string rootDirectory)
    {
        RootDirectory = rootDirectory;
    }

    public string RootDirectory { get; }

    public static AppPaths Discover(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, SolutionFile)))
            {
                return new AppPaths(directory.FullName);
            }

            directory = directory.Parent;
        }

        throw new CliException($"Could not find repository root. Expected to find {SolutionFile}.");
    }

    public string ResolveHtml(CliOptions options)
    {
        return Resolve(options.Get("html") ?? Path.Combine("schemas", "telegram", "bot-api.html"));
    }

    public string ResolveSchema(CliOptions options)
    {
        return Resolve(options.Get("schema") ?? Path.Combine("schemas", "telegram", "schema.json"));
    }

    public string ResolveOutput(CliOptions options)
    {
        return Resolve(options.Get("output") ?? Path.Combine("PPSHGram.Telegram", "Generated"));
    }

    public string Resolve(string path)
    {
        return Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(RootDirectory, path));
    }
}
