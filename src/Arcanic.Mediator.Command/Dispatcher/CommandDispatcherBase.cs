namespace Arcanic.Mediator.Command.Dispatcher;

/// <summary>
/// Abstract base class for command dispatchers.
/// Provides an interface for dispatching commands with dynamic command types and dependency injection.
/// </summary>
public abstract class CommandDispatcherBase
{
    /// <summary>
    /// Dispatches the specified command using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public abstract Task<object?> DispatchAsync(object command, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
