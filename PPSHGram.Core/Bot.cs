using PPSHGram.Core.Models.Handlers;
using PPSHGram.Core.Models.Middlewares;
using PPSHGram.Telegram;

namespace PPSHGram.Core;

public class Bot(string token) : IBot
{
    public Api Api { get; } = new Api(token);
    private IEnumerable<IMiddleware> _middlewares = new List<IMiddleware>();
    private IEnumerable<IHandler> _handlers = new List<IHandler>();
    private bool _isRunning;
    
    public void UseMiddleware(IMiddleware middleware) => _middlewares = _middlewares.Append(middleware);
    public void UseHandler(IHandler handler) => _handlers = _handlers.Append(handler);

    
    
}