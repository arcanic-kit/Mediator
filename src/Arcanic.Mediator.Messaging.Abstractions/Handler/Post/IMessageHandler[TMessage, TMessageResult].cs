namespace Arcanic.Mediator.Messaging.Abstractions.Handler.Post;

/// <summary>
/// Defines a strongly-typed post-message handler that processes specific message types and returns specific result types.
/// This interface extends the base post-handler interface while providing type safety for post-message processing.
/// </summary>
/// <typeparam name="TMessage">The type of message that this post-handler can process. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this post-handler produces. Must be a non-null reference type.</typeparam>
public interface IMessageHandler<in TMessage, out TMessageResult> : IMessagePostHandler
    where TMessage : notnull
    where TMessageResult : notnull
{
    /// <summary>
    /// Provides the non-generic implementation of post-message handling by casting the input message
    /// to the strongly-typed message type and delegating to the typed Handle method.
    /// </summary>
    /// <param name="message">The untyped message object to be post-processed.</param>
    /// <returns>The result of post-processing the message through the strongly-typed post-handler.</returns>
    object IMessagePostHandler.Handle(object message)
    {
        return Handle((TMessage)message);
    }

    /// <summary>
    /// Handles a strongly-typed message after the main handler and returns a strongly-typed result.
    /// This method contains the post-processing logic for the specific message type.
    /// </summary>
    /// <param name="message">The strongly-typed message to be post-processed by the handler.</param>
    /// <returns>The strongly-typed result of post-processing the message.</returns>
    TMessageResult Handle(TMessage message);
}