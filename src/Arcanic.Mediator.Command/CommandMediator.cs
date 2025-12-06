using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides a mediator implementation for command handling, routing commands to their appropriate handlers
/// through the underlying message mediator framework using a pipeline strategy that includes pre-handlers,
/// main handlers, and post-handlers.
/// </summary>
public class CommandMediator : ICommandMediator
{
    /// <summary>
    /// The underlying message mediator that handles the actual message routing and processing.
    /// </summary>
    private readonly IMessageMediator _messageMediator;

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandMediator"/> class.
    /// </summary>
    /// <param name="messageMediator">The message mediator instance to use for command processing.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageMediator"/> is null.</exception>
    public CommandMediator(IMessageMediator messageMediator, IServiceProvider serviceProvider)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends a command asynchronously for processing without expecting a result.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// </summary>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var strategy = new MessageMediatorRequestPipelineHandlerStrategy<ICommand>(_serviceProvider);
        var options = new MessageMediatorOptions<ICommand, Task>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        await _messageMediator.Mediate(command, options);
    }

    /// <summary>
    /// Sends a command asynchronously for processing and returns a result of the specified type.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// </summary>
    /// <typeparam name="TCommandResult">The type of result expected from the command processing.</typeparam>
    /// <param name="command">The command to send for processing that returns a result.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation, containing the command result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public async Task<TCommandResult> SendAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var strategy = new MessageMediatorRequestPipelineHandlerStrategy<ICommand<TCommandResult>, TCommandResult>(_serviceProvider);
        var options = new MessageMediatorOptions<ICommand<TCommandResult>, Task<TCommandResult>>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        return await _messageMediator.Mediate(command, options);
    }
}