using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core.Models.Context;

public interface IContext
{
    public Api Api { get; }

    public Update Update { get; }
}
