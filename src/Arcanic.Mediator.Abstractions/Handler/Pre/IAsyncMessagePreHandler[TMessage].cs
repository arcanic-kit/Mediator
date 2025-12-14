namespace Arcanic.Mediator.Abstractions.Handler.Pre;

/// <summary>
/// Defines an asynchronous pre-message handler that processes messages before the main handler without returning a specific result.
/// This interface extends the base message handler to support asynchronous operations with cancellation support.
/// </summary>
/// <typeparam name="TMessage">The type of message that this pre-handler can process. Must be a non-null reference type.</typeparam>
public interface IAsyncMessagePreHandler<TMessage> : IMessageHandler<TMessage> where TMessage : notnull
{
    /// <summary>
    /// Provides the synchronous interface implementation by delegating to the asynchronous pre-handler method.
    /// This method retrieves the cancellation token from the current mediator context and passes it to the async pre-handler.
    /// </summary>
    /// <param name="message">The message to be processed by the pre-handler.</param>
    /// <returns>A task representing the asynchronous message pre-processing operation.</returns>
    object IMessageHandler<TMessage>.Handle(TMessage message)
    {
        return HandleAsync(message, default);
    }

    /// <summary>
    /// Asynchronously handles a message before the main handler with support for cancellation.
    /// This method contains the pre-processing logic that should be executed before the main handler.
    /// </summary>
    /// <param name="message">The message to be pre-processed by the handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. Defaults to CancellationToken.None.</param>
    /// <returns>A task representing the asynchronous message pre-processing operation.</returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}