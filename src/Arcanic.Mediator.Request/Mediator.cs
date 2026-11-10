using System.Collections.Concurrent;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Request;

/// <summary>
/// Default <see cref="IMediator"/> implementation. Resolves the dispatcher for a request type once, caches it,
/// and runs the request through it using the service provider the mediator was resolved from.
/// </summary>
public class Mediator : IMediator
{
    /// <summary>
    /// Process-wide cache of dispatchers keyed by request type. Dispatchers are stateless, so one instance per
    /// request type is shared by every <see cref="Mediator"/> regardless of its DI lifetime.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, RequestDispatcherBase> Dispatchers = new();

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve handlers and pipeline behaviors.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is <c>null</c>.</exception>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var result = await DispatchAsync(request, cancellationToken).ConfigureAwait(false);

        return (TResponse)result!;
    }

    /// <inheritdoc />
    public Task SendAsync(IRequest request, CancellationToken cancellationToken = default) =>
        DispatchAsync(request, cancellationToken);

    /// <summary>
    /// Runs the request through the dispatcher for its concrete type.
    /// </summary>
    private Task<object?> DispatchAsync(IRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var dispatcher = Dispatchers.GetOrAdd(request.GetType(), static (requestType, serviceProvider) =>
            CreateDispatcher(requestType, serviceProvider), _serviceProvider);

        return dispatcher.DispatchAsync(request, _serviceProvider, cancellationToken);
    }

    /// <summary>
    /// Asks each registered <see cref="IRequestDispatcherFactory"/> for a dispatcher. Only runs on a cache miss.
    /// </summary>
    private static RequestDispatcherBase CreateDispatcher(Type requestType, IServiceProvider serviceProvider)
    {
        foreach (var factory in serviceProvider.GetServices<IRequestDispatcherFactory>())
        {
            if (factory.CreateDispatcher(requestType) is { } dispatcher)
            {
                return dispatcher;
            }
        }

        throw new InvalidOperationException(
            $"No dispatcher is available for request type '{requestType.FullName}'. " +
            "Register the module that handles it, for example with AddCommands or AddQueries.");
    }
}
