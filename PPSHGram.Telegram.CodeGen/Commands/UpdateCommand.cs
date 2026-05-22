using PPSHGram.Telegram.CodeGen.Cli;

namespace PPSHGram.Telegram.CodeGen.Commands;

internal sealed class UpdateCommand(AppPaths paths, CliOptions options) : ICommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await new FetchCommand(paths, options).ExecuteAsync(cancellationToken);
        await new ParseCommand(paths, options).ExecuteAsync(cancellationToken);
        await new GenerateCommand(paths, options).ExecuteAsync(cancellationToken);
    }
}
