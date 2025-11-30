using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Abstractions.Handler.Post;

/// <summary>
/// Defines an asynchronous post-message handler that processes messages and results after the main handler with support for typed results.
/// This interface extends the base message handler to support asynchronous operations with cancellation support.
/// </summary>
/// <typeparam name="TMessage">The type of message that this post-handler can process. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this post-handler can access from the main handler.</typeparam>
public interface IAsyncMessagePostHandler<TMessage, TMessageResult> : IMessageHandler<TMessage, Task> where TMessage : notnull
{
    /// <summary>
    /// Provides the synchronous interface implementation by delegating to the asynchronous post-handler method.
    /// This method retrieves the cancellation token from the current mediator context and passes it to the async post-handler.
    /// Note: The result parameter is not available in this synchronous interface and will be default.
    /// </summary>
    /// <param name="message">The message to be processed by the post-handler.</param>
    /// <returns>A task representing the asynchronous message post-processing operation.</returns>
    Task IMessageHandler<TMessage, Task>.Handle(TMessage message)
    {
        return HandleAsync(message, default(TMessageResult)!, MessageMediatorContextAccessor.Current.CancellationToken);
    }

    /// <summary>
    /// Asynchronously handles a message and result after the main handler with support for cancellation.
    /// This method contains the post-processing logic that should be executed after the main handler,
    /// with access to both the original message and the result produced by the main handler.
    /// </summary>
    /// <param name="message">The message that was processed by the main handler.</param>
    /// <param name="result">The result produced by the main handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. Defaults to CancellationToken.None.</param>
    /// <returns>A task representing the asynchronous message post-processing operation.</returns>
    Task HandleAsync(TMessage message, TMessageResult result, CancellationToken cancellationToken = default);
}