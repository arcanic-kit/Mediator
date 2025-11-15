namespace Arcanic.Mediator.Event.Abstractions;

/// <summary>
/// Defines a contract for publishing events within the mediator framework.
/// </summary>
public interface IEventMediator
{
    /// <summary>
    /// Asynchronously publishes an event to all registered handlers.
    /// </summary>
    /// <param name="request">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous publishing operation.</returns>
    Task PublishAsync(IEvent request, CancellationToken cancellationToken = default);
}
