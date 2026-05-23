using PPSHGram.Core.Models.Context;

namespace PPSHGram.Core.Models.Handlers;

public interface IHandler
{
    Task Handle(IContext context, CancellationToken cancellationToken = default);
}
