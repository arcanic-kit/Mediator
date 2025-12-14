using Arcanic.Mediator.Abstractions.Handler.Pre;

namespace Arcanic.Mediator.Event.Abstractions;

/// <summary>
/// Defines a pre-handler that executes before the main event handlers.
/// Pre-handlers are useful for implementing cross-cutting concerns such as validation,
/// authentication, logging, or other preparatory operations.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle, which must implement <see cref="IEvent"/>.</typeparam>
public interface IEventPreHandler<TEvent> : IAsyncMessagePreHandler<TEvent> where TEvent : IEvent;