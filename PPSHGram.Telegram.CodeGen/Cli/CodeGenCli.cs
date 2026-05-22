using PPSHGram.Telegram.CodeGen.Commands;

namespace PPSHGram.Telegram.CodeGen.Cli;

internal static class CodeGenCli
{
    public static async Task<int> RunAsync(string[] args, CancellationToken cancellationToken)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            PrintHelp();
            return 0;
        }

        var commandName = args[0].Trim().ToLowerInvariant();
        var options = CliOptions.Parse(args.Skip(1));
        var paths = AppPaths.Discover(Environment.CurrentDirectory);

        try
        {
            ICommand command = commandName switch
            {
                "fetch" => new FetchCommand(paths, options),
                "parse" => new ParseCommand(paths, options),
                "generate" => new GenerateCommand(paths, options),
                "update" => new UpdateCommand(paths, options),
                _ => throw new CliException($"Unknown command '{args[0]}'.")
            };

            await command.ExecuteAsync(cancellationToken);
            return 0;
        }
        catch (CliException exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 2;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static bool IsHelp(string value)
    {
        return value is "-h" or "--help" or "help";
    }

    private static void PrintHelp()
    {
        Console.WriteLine(
            """
            PPSHGram.Telegram.CodeGen

            Commands:
              fetch      Download Telegram Bot API HTML.
              parse      Parse local HTML into schema JSON.
              generate   Generate C# files from schema JSON.
              update     Run fetch, parse, then generate.

            Defaults:
              --url      https://core.telegram.org/bots/api
              --html     schemas/telegram/bot-api.html
              --schema   schemas/telegram/schema.json
              --output   PPSHGram.Telegram/Generated

            Examples:
              dotnet run --project PPSHGram.Telegram.CodeGen -- fetch
              dotnet run --project PPSHGram.Telegram.CodeGen -- parse
              dotnet run --project PPSHGram.Telegram.CodeGen -- generate
              dotnet run --project PPSHGram.Telegram.CodeGen -- update
            """);
    }
}
