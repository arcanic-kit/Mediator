using Arcanic.Mediator.Command.Abstractions;
using System.Collections.Concurrent;
using Arcanic.Mediator.Command.Handler;

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
    /// A thread-safe cache mapping command types to their corresponding handler wrapper instances.
    /// This avoids repeated allocations and reflection for each command execution.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, CommandHandlerWrapperBase> CommandHandlers = new();

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
    /// Uses cached strategy instances to improve performance.
    /// </summary>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();

        // Retrieve or create a handler wrapper for the command type.
        var handler = (CommandHandlerWrapper)CommandHandlers.GetOrAdd(commandType, static requestType =>
        {
            var wrapperType = typeof(CommandHandlerWrapperImpl<>).MakeGenericType(requestType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (CommandHandlerWrapperBase)wrapper;
        });

        // Delegate the handling of the command to the resolved handler.
        return handler.Handle(command, _serviceProvider, cancellationToken);
    }

    /// <summary>
    /// Sends a command asynchronously for processing and expects a result of type <typeparamref name="TCommandResponse"/>.
    /// The command is routed through a complete pipeline including pre-handlers, the main handler, and post-handlers.
    /// Uses cached strategy instances to improve performance.
    /// </summary>
    /// <typeparam name="TCommandResponse">The type of the response expected from the command.</typeparam>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation, containing the command response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public Task<TCommandResponse> SendAsync<TCommandResponse>(ICommand<TCommandResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();

        // Retrieve or create a handler wrapper for the command type.
        var handler = (CommandHandlerWrapper<TCommandResponse>)CommandHandlers.GetOrAdd(commandType, static requestType =>
        {
            var wrapperType = typeof(CommandHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TCommandResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (CommandHandlerWrapperBase)wrapper;
        });

        // Delegate the handling of the command to the resolved handler.
        return handler.Handle(command, _serviceProvider, cancellationToken);
    }
}
