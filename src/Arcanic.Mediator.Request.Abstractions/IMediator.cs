using Arcanic.Mediator.Request.Abstractions.Dispatcher;
using System.Collections.Concurrent;

namespace Arcanic.Mediator.Request.Abstractions;

/// <summary>
/// Defines the contract for a mediator that orchestrates request processing and dispatcher management.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Gets the service provider used for dependency resolution.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets a thread-safe dictionary of request dispatchers keyed by their associated request types.
    /// </summary>
    ConcurrentDictionary<Type, RequestDispatcherBase> RequestDispatchers { get; }
}
