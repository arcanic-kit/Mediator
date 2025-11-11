using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;

namespace Arcanic.Mediator.Command;

public class CommandMediator : ICommandMediator
{
    private readonly IMessageMediator _messageMediator;

    public CommandMediator(IMessageMediator messageMediator)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
    }

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var strategy = new MessageMediatorSingleMainHandlerStrategy<ICommand>();
        var options = new MessageMediatorOptions<ICommand, Task>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        await _messageMediator.Mediate(command, options);
    }

    public async Task<TCommandResult> SendAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var strategy = new MessageMediatorSingleMainHandlerStrategy<ICommand<TCommandResult>, TCommandResult>();
        var options = new MessageMediatorOptions<ICommand<TCommandResult>, Task<TCommandResult>>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        return await _messageMediator.Mediate(command, options);
    }
}