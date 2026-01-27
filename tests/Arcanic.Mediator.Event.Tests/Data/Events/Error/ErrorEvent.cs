using Arcanic.Mediator.Event.Abstractions;

namespace Arcanic.Mediator.Event.Tests.Data.Events.Error;

public class ErrorEvent : IEvent
{
    public string ErrorMessage { get; init; } = string.Empty;
}