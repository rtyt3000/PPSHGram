using System.Reflection;
using PPSHGram.Core.Models.Handlers;
using PPSHGram.Core.Models.Middlewares;
using PPSHGram.Core.Models.Polling;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core;

public interface IBot
{
    Api Api { get; }

    bool IsRunning { get; }

    void UseMiddleware(IMiddleware middleware);

    void UseMiddleware<TMiddleware>()
        where TMiddleware : IMiddleware;

    void UseHandler(IHandler handler);

    void UseHandler<THandler>();

    void UseHandler(Type handlerType);

    void UseHandlers(Assembly assembly);

    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken = default);

    Task StartPolling(CancellationToken cancellationToken = default);

    Task StartPolling(PollingOptions options, CancellationToken cancellationToken = default);
}
