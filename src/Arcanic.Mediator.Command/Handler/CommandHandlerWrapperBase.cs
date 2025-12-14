namespace Arcanic.Mediator.Command.Handler;

/// <summary>
/// Abstract base class for query handler wrappers.
/// Provides an interface for handling queries with dynamic request types and dependency injection.
/// </summary>
public abstract class CommandHandlerWrapperBase
{
    /// <summary>
    /// Handles the specified query request using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request">The query request to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}