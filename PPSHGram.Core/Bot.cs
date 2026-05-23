using PPSHGram.Core.Models.Handlers;
using PPSHGram.Core.Models.Middlewares;
using PPSHGram.Telegram;
using System.Reflection;

namespace PPSHGram.Core;

public class Bot(string token) : IBot
{
    public Api Api { get; } = new Api(token);
    private IEnumerable<IMiddleware> _middlewares = new List<IMiddleware>();
    private IEnumerable<IHandler> _handlers = new List<IHandler>();
    private IEnumerable<HandlerDescriptor> _handlerDescriptors = new List<HandlerDescriptor>();
    private bool _isRunning;
    
    public void UseMiddleware(IMiddleware middleware) => _middlewares = _middlewares.Append(middleware);
    public void UseHandler(IHandler handler) => _handlers = _handlers.Append(handler);
    public void UseHandler<THandler>() => UseHandler(typeof(THandler));
    public void UseHandler(Type handlerType) => _handlerDescriptors = _handlerDescriptors.Concat(HandlerDiscovery.Discover(handlerType));
    public void UseHandlers(Assembly assembly) => _handlerDescriptors = _handlerDescriptors.Concat(HandlerDiscovery.Discover(assembly));

    
    
}
