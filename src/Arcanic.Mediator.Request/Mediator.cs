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
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    public IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// Gets or sets a thread-safe dictionary of request dispatchers keyed by their associated request types.
    /// </summary>
    public ConcurrentDictionary<Type, RequestDispatcherBase> RequestDispatchers { get; set; } = new();

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
