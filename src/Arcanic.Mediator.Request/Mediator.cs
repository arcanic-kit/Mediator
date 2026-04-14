using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Dispatcher;
using System.Collections.Concurrent;

namespace Arcanic.Mediator.Request;

/// <summary>
/// Provides a concrete implementation of the mediator pattern for orchestrating request processing and dispatcher management.
/// </summary>
public class Mediator : IMediator
{
    /// <summary>
    /// A process-wide, thread-safe cache mapping request types to their corresponding dispatcher instances.
    /// Shared across all <see cref="Mediator"/> instances so that dispatcher creation via reflection
    /// occurs only once per request type for the lifetime of the application, regardless of the
    /// configured DI lifetime (transient, scoped, or singleton).
    /// </summary>
    private static readonly ConcurrentDictionary<Type, RequestDispatcherBase> _requestDispatchers = new();
    
    /// <summary>
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets a thread-safe dictionary of request dispatchers keyed by their associated request types.
    /// </summary>
    public ConcurrentDictionary<Type, RequestDispatcherBase> RequestDispatchers { get; } = _requestDispatchers;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider used for dependency injection and handler resolution.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
    public Mediator(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
}
