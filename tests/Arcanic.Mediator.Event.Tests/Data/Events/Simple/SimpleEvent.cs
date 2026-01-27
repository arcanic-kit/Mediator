using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Tests.Data.Events.Simple;

public class SimpleEvent : IEvent
{
    public int Value { get; init; }
    public string Message { get; init; } = string.Empty;
}