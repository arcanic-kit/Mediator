using Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Event.Abstractions;

public interface IEventHandler<TEvent> : IAsyncMessageMainHandler<TEvent> where TEvent : IEvent;

