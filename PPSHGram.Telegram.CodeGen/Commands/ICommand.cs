namespace PPSHGram.Telegram.CodeGen.Commands;

internal interface ICommand
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
