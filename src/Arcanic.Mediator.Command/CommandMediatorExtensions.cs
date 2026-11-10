using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Request.Abstractions;

/// <summary>
/// Provides typed command-sending methods on <see cref="IMediator"/>.
/// </summary>
public static class CommandMediatorExtensions
{
    /// <summary>
    /// Sends a command that produces no result through its pipeline: pipeline behaviors, pre-handlers,
    /// the command handler and post-handlers.
    /// </summary>
    /// <param name="mediator">The mediator to send the command through.</param>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that completes when the command has been handled.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mediator"/> or <paramref name="command"/> is <c>null</c>.</exception>
    public static Task SendAsync(this IMediator mediator, ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(command);

        return mediator.DispatchAsync(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command that produces a result through its pipeline: pipeline behaviors, pre-handlers,
    /// the command handler and post-handlers.
    /// </summary>
    /// <typeparam name="TCommandResponse">The type of result the command produces.</typeparam>
    /// <param name="mediator">The mediator to send the command through.</param>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task whose result is the command handler's response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mediator"/> or <paramref name="command"/> is <c>null</c>.</exception>
    public static async Task<TCommandResponse> SendAsync<TCommandResponse>(this IMediator mediator, ICommand<TCommandResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(command);

        var result = await mediator.DispatchAsync(command, cancellationToken).ConfigureAwait(false);

        return (TCommandResponse)result!;
    }
}
