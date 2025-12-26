using System.Collections.Concurrent;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Dispatcher;

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
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// A thread-safe cache mapping command types to their corresponding dispatcher instances.
    /// This avoids repeated allocations and reflection for each command execution.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, CommandDispatcherBase> CommandDispatchers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandMediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies and handlers.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is null.</exception>
    public CommandMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Sends a command asynchronously for processing without expecting a result.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// Uses cached dispatcher instances to improve performance.
    /// </summary>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();

        // Retrieve or create a dispatcher for the command type.
        var dispatcher = CommandDispatchers.GetOrAdd(commandType, static requestType =>
        {
            var dispatcherType = typeof(CommandDispatcher<>).MakeGenericType(requestType);
            var dispatcherInstance = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (CommandDispatcherBase)dispatcherInstance;
        });

        // Delegate the dispatching of the command to the resolved dispatcher.
        await dispatcher.DispatchAsync(command, _serviceProvider, cancellationToken);
    }

    /// <summary>
    /// Sends a command asynchronously for processing and expects a result of type <typeparamref name="TCommandResponse"/>.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// Uses cached dispatcher instances to improve performance.
    /// </summary>
    /// <typeparam name="TCommandResponse">The type of the response expected from the command.</typeparam>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation, containing the command response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public async Task<TCommandResponse> SendAsync<TCommandResponse>(ICommand<TCommandResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();

        // Retrieve or create a dispatcher for the command type.
        var dispatcher = CommandDispatchers.GetOrAdd(commandType, static requestType =>
        {
            var dispatcherType = typeof(CommandDispatcher<,>).MakeGenericType(requestType, typeof(TCommandResponse));
            var dispatcherInstance = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (CommandDispatcherBase)dispatcherInstance;
        });

        // Delegate the dispatching of the command to the resolved dispatcher.
        var result = await dispatcher.DispatchAsync(command, _serviceProvider, cancellationToken);
        return (TCommandResponse)result!;
    }
}
