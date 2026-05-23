using PPSHGram.Telegram;

namespace PPSHGram.Core.Models.Context;

public interface IContext
{
    public Api Api { get; }
}