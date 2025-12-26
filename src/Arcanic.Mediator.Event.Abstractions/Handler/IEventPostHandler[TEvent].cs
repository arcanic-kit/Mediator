using Arcanic.Mediator.Abstractions.Handler.Post;

namespace Arcanic.Mediator.Event.Abstractions.Handler;

/// <summary>
/// Defines a post-handler that executes after the main event handlers.
/// Post-handlers are useful for implementing cross-cutting concerns such as logging,
/// cleanup operations, metrics collection, or other follow-up activities.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle, which must implement <see cref="IEvent"/>.</typeparam>
public interface IEventPostHandler<TEvent> : IAsyncMessagePostHandler<TEvent> where TEvent : IEvent;