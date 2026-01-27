using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Tests.Data.Events.Cancellable;

public class CancellableEvent : IEvent
{
    public int DelayMilliseconds { get; init; }
}