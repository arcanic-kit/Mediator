using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Dispatcher;
using Arcanic.Mediator.Request.Abstractions;

namespace Arcanic.Mediator.Command;

/// <summary>
/// Provides extension methods for <see cref="IMediator"/> to support query processing functionality.
/// </summary>
public static class MediatorExtensions
{
    /// <summary>
    /// Sends a command asynchronously for processing without expecting a result.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// Uses cached dispatcher instances to improve performance.
    /// </summary>
    /// <param name="mediator">The mediator instance to send the query through.</param>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public static async Task SendAsync(this IMediator mediator, ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();

        // Retrieve or create a dispatcher for the command type.
        var dispatcher = mediator.RequestDispatchers.GetOrAdd(commandType, static requestType =>
        {
            var dispatcherType = typeof(CommandDispatcher<>).MakeGenericType(requestType);
            var dispatcherInstance = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (CommandDispatcherBase)dispatcherInstance;
        });

        // Delegate the dispatching of the command to the resolved dispatcher.
        await dispatcher.DispatchAsync(command, mediator.ServiceProvider, cancellationToken);
    }

    /// <summary>
    /// Sends a command asynchronously for processing and expects a result of type <typeparamref name="TCommandResponse"/>.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// Uses cached dispatcher instances to improve performance.
    /// </summary>
    /// <typeparam name="TCommandResponse">The type of the response expected from the command.</typeparam>
    /// <param name="mediator">The mediator instance to send the query through.</param>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation, containing the command response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public static async Task<TCommandResponse> SendAsync<TCommandResponse>(this IMediator mediator, ICommand<TCommandResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();

        // Retrieve or create a dispatcher for the command type.
        var dispatcher = mediator.RequestDispatchers.GetOrAdd(commandType, static requestType =>
        {
            var dispatcherType = typeof(CommandDispatcher<,>).MakeGenericType(requestType, typeof(TCommandResponse));
            var dispatcherInstance = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (CommandDispatcherBase)dispatcherInstance;
        });

        // Delegate the dispatching of the command to the resolved dispatcher.
        var result = await dispatcher.DispatchAsync(command, mediator.ServiceProvider, cancellationToken);
        return (TCommandResponse)result!;
    }
}
