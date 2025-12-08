using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;
using System.Collections.Concurrent;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides a mediator implementation for command handling, routing commands to their appropriate handlers
/// through the underlying message mediator framework using a pipeline strategy that includes pre-handlers,
/// main handlers, and post-handlers.
/// Optimized version that caches strategy instances to avoid repeated allocations.
/// </summary>
public class CommandMediator : ICommandMediator
{
    /// <summary>
    /// The underlying message mediator responsible for coordinating command processing and handler invocation.
    /// </summary>
    private readonly IMessageMediator _messageMediator;

    /// <summary>
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Cache for void command strategies to avoid repeated allocations.
    /// </summary>
    private readonly ConcurrentDictionary<Type, MessageMediatorRequestPipelineHandlerStrategy<ICommand>> _voidStrategyCache = new();
    
    /// <summary>
    /// Cache for result command strategies to avoid repeated allocations.
    /// </summary>
    private readonly ConcurrentDictionary<Type, object> _resultStrategyCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandMediator"/> class.
    /// </summary>
    /// <param name="messageMediator">The message mediator instance to use for command processing.</param>
    /// <param name="serviceProvider">The service provider used for dependency injection and handler resolution.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageMediator"/> or <paramref name="serviceProvider"/> is null.</exception>
    public CommandMediator(IMessageMediator messageMediator, IServiceProvider serviceProvider)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends a command asynchronously for processing without expecting a result.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// Uses cached strategy instances to improve performance.
    /// </summary>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public ValueTask SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();
        var strategy = _voidStrategyCache.GetOrAdd(commandType, 
            _ => new MessageMediatorRequestPipelineHandlerStrategy<ICommand>(_serviceProvider));

        var options = new MessageMediatorOptions<ICommand, Task>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        return new ValueTask(_messageMediator.Mediate(command, options));
    }

    /// <summary>
    /// Sends a command asynchronously for processing and returns a result of the specified type.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// Uses cached strategy instances to improve performance.
    /// </summary>
    /// <typeparam name="TCommandResult">The type of result expected from the command processing.</typeparam>
    /// <param name="command">The command to send for processing that returns a result.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation, containing the command result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public ValueTask<TCommandResult> SendAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();
        var strategy = (MessageMediatorRequestPipelineHandlerStrategy<ICommand<TCommandResult>, TCommandResult>)
            _resultStrategyCache.GetOrAdd(commandType, 
                _ => new MessageMediatorRequestPipelineHandlerStrategy<ICommand<TCommandResult>, TCommandResult>(_serviceProvider));

        var options = new MessageMediatorOptions<ICommand<TCommandResult>, Task<TCommandResult>>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        return new ValueTask<TCommandResult>(_messageMediator.Mediate(command, options));
    }
}