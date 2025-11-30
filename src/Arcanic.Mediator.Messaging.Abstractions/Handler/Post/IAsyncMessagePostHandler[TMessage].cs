using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Abstractions.Handler.Post;

/// <summary>
/// Defines an asynchronous post-message handler that processes messages after the main handler without returning a specific result.
/// This interface extends the base message handler to support asynchronous operations with cancellation support.
/// </summary>
/// <typeparam name="TMessage">The type of message that this post-handler can process. Must be a non-null reference type.</typeparam>
public interface IAsyncMessagePostHandler<TMessage> : IMessageHandler<TMessage> where TMessage : notnull
{
    /// <summary>
    /// Provides the synchronous interface implementation by delegating to the asynchronous post-handler method.
    /// This method retrieves the cancellation token from the current mediator context and passes it to the async post-handler.
    /// </summary>
    /// <param name="message">The message to be processed by the post-handler.</param>
    /// <returns>A task representing the asynchronous message post-processing operation.</returns>
    object IMessageHandler<TMessage>.Handle(TMessage message)
    {
        return HandleAsync(message, MessageMediatorContextAccessor.Current.CancellationToken);
    }

    /// <summary>
    /// Asynchronously handles a message after the main handler with support for cancellation.
    /// This method contains the post-processing logic that should be executed after the main handler.
    /// </summary>
    /// <param name="message">The message to be post-processed by the handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. Defaults to CancellationToken.None.</param>
    /// <returns>A task representing the asynchronous message post-processing operation.</returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}