namespace Arcanic.Mediator.Event.Abstractions;

public interface IEventMediator
{
    Task PublishAsync(IEvent request, CancellationToken cancellationToken = default);
}
