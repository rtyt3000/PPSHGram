using PPSHGram.Core.Models.Context;

namespace PPSHGram.Core.Models.Middlewares;

public delegate Task MiddlewareHandler(IContext context, CancellationToken cancellationToken = default);

public interface IMiddleware
{
    Task Handle(IContext context, MiddlewareHandler next, CancellationToken cancellationToken = default);
}