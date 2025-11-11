using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;

namespace Arcanic.Mediator.Event;

public class EventMediator : IEventMediator
{
    private readonly IMessageMediator _messageMediator;

    public EventMediator(IMessageMediator messageMediator)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
    }

    public async Task PublishAsync(IEvent request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var strategy = new MessageMediatorMultipleMainHandlerStrategy<IEvent>();
        var options = new MessageMediatorOptions<IEvent, Task>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        await _messageMediator.Mediate(request, options);
    }
}
