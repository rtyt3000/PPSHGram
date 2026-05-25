using System.Reflection;
using System.Runtime.ExceptionServices;
using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;
using PPSHGram.Core.Models.Handlers;
using PPSHGram.Core.Models.Middlewares;
using PPSHGram.Core.Models.Polling;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Requests;
using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core;

public class Bot : IBot, IDisposable
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly bool _ownsApi;
    private readonly List<IMiddleware> _middlewares = [];
    private readonly List<HandlerRegistration> _handlers = [];
    private bool _isRunning;

    public Bot(string token, IServiceProvider? serviceProvider = null)
        : this(new Api(token), serviceProvider)
    {
        _ownsApi = true;
    }

    public Bot(Api api, IServiceProvider? serviceProvider = null)
    {
        Api = api ?? throw new ArgumentNullException(nameof(api));
        _serviceProvider = serviceProvider;
    }

    public Api Api { get; }

    public bool IsRunning => _isRunning;

    public void UseMiddleware(IMiddleware middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        _middlewares.Add(middleware);
    }

    public void UseMiddleware<TMiddleware>()
        where TMiddleware : IMiddleware
    {
        UseMiddleware((TMiddleware)CreateInstance(typeof(TMiddleware)));
    }

    public void UseHandler(IHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _handlers.Add(new HandlerRegistration(handler));
    }

    public void UseHandler<THandler>()
    {
        UseHandler(typeof(THandler));
    }

    public void UseHandler(Type handlerType)
    {
        _handlers.AddRange(HandlerDiscovery.Discover(handlerType).Select(descriptor => new HandlerRegistration(descriptor)));
    }

    public void UseHandlers(Assembly assembly)
    {
        _handlers.AddRange(HandlerDiscovery.Discover(assembly).Select(descriptor => new HandlerRegistration(descriptor)));
    }

    public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(update);

        var context = UpdateContextFactory.Create(Api, update);
        return ExecutePipeline(context, cancellationToken);
    }

    public Task StartPolling(CancellationToken cancellationToken = default)
    {
        return StartPolling(new PollingOptions(), cancellationToken);
    }

    public async Task StartPolling(
        PollingOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (_isRunning)
        {
            throw new InvalidOperationException("Bot polling is already running.");
        }

        _isRunning = true;
        var offset = options.Offset;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var updates = await Api.GetUpdates(new GetUpdatesRequest
                {
                    Offset = offset,
                    Limit = options.Limit,
                    Timeout = options.Timeout,
                    AllowedUpdates = options.AllowedUpdates
                }, cancellationToken).ConfigureAwait(false);

                foreach (var update in updates)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await HandleUpdateAsync(update, cancellationToken).ConfigureAwait(false);
                    if (offset is null || update.UpdateId >= offset.Value)
                    {
                        offset = update.UpdateId + 1;
                    }
                }
            }
        }
        finally
        {
            _isRunning = false;
        }
    }

    public void Dispose()
    {
        if (_ownsApi)
        {
            Api.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    private Task ExecutePipeline(IContext context, CancellationToken cancellationToken)
    {
        MiddlewareHandler next = DispatchHandlers;

        for (var index = _middlewares.Count - 1; index >= 0; index--)
        {
            var middleware = _middlewares[index];
            var currentNext = next;
            next = (ctx, ct) => middleware.Handle(ctx, currentNext, ct);
        }

        return next(context, cancellationToken);
    }

    private async Task DispatchHandlers(IContext context, CancellationToken cancellationToken = default)
    {
        foreach (var registration in _handlers)
        {
            if (registration.DirectHandler is not null)
            {
                await registration.DirectHandler.Handle(context, cancellationToken).ConfigureAwait(false);
                return;
            }

            var descriptor = registration.Descriptor!;
            if (!descriptor.ContextType.IsInstanceOfType(context)
                || !HandlerFilterEvaluator.Matches(context, descriptor.Filters))
            {
                continue;
            }

            var instance = CreateInstance(descriptor.HandlerType);
            await InvokeHandler(instance, descriptor.Method, context, cancellationToken).ConfigureAwait(false);
            return;
        }
    }

    private async Task InvokeHandler(
        object instance,
        MethodInfo method,
        IContext context,
        CancellationToken cancellationToken)
    {
        var arguments = BindParameters(method, context, cancellationToken);
        object? result;
        try
        {
            result = method.Invoke(instance, arguments);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }

        await AwaitHandlerResult(result).ConfigureAwait(false);
    }

    private object?[] BindParameters(
        MethodInfo method,
        IContext context,
        CancellationToken cancellationToken)
    {
        var parameters = method.GetParameters();
        var arguments = new object?[parameters.Length];

        for (var index = 0; index < parameters.Length; index++)
        {
            arguments[index] = BindParameter(parameters[index], method, context, cancellationToken);
        }

        return arguments;
    }

    private object? BindParameter(
        ParameterInfo parameter,
        MethodInfo method,
        IContext context,
        CancellationToken cancellationToken)
    {
        if (parameter.ParameterType.IsInstanceOfType(context))
        {
            return context;
        }

        if (parameter.ParameterType == typeof(CancellationToken))
        {
            return cancellationToken;
        }

        if (parameter.ParameterType.IsInstanceOfType(Api))
        {
            return Api;
        }

        if (parameter.ParameterType.IsInstanceOfType(this))
        {
            return this;
        }

        var service = _serviceProvider?.GetService(parameter.ParameterType);
        if (service is not null)
        {
            return service;
        }

        if (parameter.HasDefaultValue)
        {
            return parameter.DefaultValue;
        }

        throw new InvalidOperationException(
            $"Cannot bind parameter {parameter.Name} of handler {method.DeclaringType?.Name}.{method.Name}.");
    }

    private object CreateInstance(Type type)
    {
        var service = _serviceProvider?.GetService(type);
        if (service is not null)
        {
            return service;
        }

        foreach (var constructor in type
                     .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                     .OrderByDescending(constructor => constructor.GetParameters().Length))
        {
            if (TryCreateConstructorArguments(constructor, out var arguments))
            {
                return constructor.Invoke(arguments);
            }
        }

        throw new InvalidOperationException($"Cannot create an instance of {type.FullName}.");
    }

    private bool TryCreateConstructorArguments(ConstructorInfo constructor, out object?[] arguments)
    {
        var parameters = constructor.GetParameters();
        arguments = new object?[parameters.Length];

        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            if (parameter.ParameterType.IsInstanceOfType(Api))
            {
                arguments[index] = Api;
                continue;
            }

            if (parameter.ParameterType.IsInstanceOfType(this))
            {
                arguments[index] = this;
                continue;
            }

            if (parameter.ParameterType == typeof(IServiceProvider))
            {
                arguments[index] = _serviceProvider;
                continue;
            }

            var service = _serviceProvider?.GetService(parameter.ParameterType);
            if (service is not null)
            {
                arguments[index] = service;
                continue;
            }

            if (parameter.HasDefaultValue)
            {
                arguments[index] = parameter.DefaultValue;
                continue;
            }

            arguments = [];
            return false;
        }

        return true;
    }

    private static async Task AwaitHandlerResult(object? result)
    {
        switch (result)
        {
            case null:
                return;
            case Task task:
                await task.ConfigureAwait(false);
                return;
            case ValueTask valueTask:
                await valueTask.ConfigureAwait(false);
                return;
        }

        var resultType = result.GetType();
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            var task = (Task?)resultType.GetMethod(nameof(ValueTask.AsTask))?.Invoke(result, []);
            if (task is not null)
            {
                await task.ConfigureAwait(false);
            }
        }
    }

    private sealed record HandlerRegistration(IHandler? DirectHandler, HandlerDescriptor? Descriptor)
    {
        public HandlerRegistration(IHandler directHandler)
            : this(directHandler, null)
        {
        }

        public HandlerRegistration(HandlerDescriptor descriptor)
            : this(null, descriptor)
        {
        }
    }
}
