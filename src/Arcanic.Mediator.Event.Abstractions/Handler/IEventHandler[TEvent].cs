using Arcanic.Mediator.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Event.Abstractions.Handler;

/// <summary>
/// Defines a contract for handling events of a specific type within the mediator framework.
/// Event handlers implement the publish-subscribe pattern, where multiple handlers can
/// process the same event independently and asynchronously.
/// </summary>
public interface IEventHandler<TEvent> : IAsyncMessageMainHandler<TEvent> where TEvent : IEvent;

