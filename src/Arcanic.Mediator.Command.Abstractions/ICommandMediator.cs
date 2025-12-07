namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Defines a mediator for sending commands to their appropriate handlers.
/// The command mediator provides a decoupled way to dispatch commands without requiring
/// direct dependencies on specific command handlers.
/// </summary>
public interface ICommandMediator
{
    /// <summary>
    /// Sends a command asynchronously for processing without expecting a result.
    /// The command will be routed to its designated handler for execution.
    /// </summary>
    /// <param name="command">The command to send for processing.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    ValueTask SendAsync(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command asynchronously for processing and returns a result of the specified type.
    /// The command will be routed to its designated handler for execution.
    /// </summary>
    /// <typeparam name="TCommandResult">The type of result expected from the command processing.</typeparam>
    /// <param name="command">The command to send for processing that returns a result.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during command processing.</param>
    /// <returns>A task that represents the asynchronous command processing operation, containing the command result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    ValueTask<TCommandResult> SendAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default);
}