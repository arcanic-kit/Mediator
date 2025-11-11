namespace Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

/// <summary>
/// Defines a base interface for main message handlers that can process messages of any type.
/// This interface provides a non-generic entry point for message handling within the mediator framework.
/// </summary>
public interface IMessageMainHandler
{
    /// <summary>
    /// Handles a message and returns the processing result.
    /// This method accepts an untyped message object and returns an untyped result.
    /// </summary>
    /// <param name="message">The message object to be processed by the handler.</param>
    /// <returns>The result of processing the message.</returns>
    object Handle(object message);
}

/// <summary>
/// Defines a strongly-typed main message handler that processes specific message types and returns specific result types.
/// This interface extends the base handler interface while providing type safety for message processing.
/// </summary>
/// <typeparam name="TMessage">The type of message that this handler can process. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this handler produces. Must be a non-null reference type.</typeparam>
public interface IMessageHandler<in TMessage, out TMessageResult> : IMessageMainHandler
    where TMessage : notnull
    where TMessageResult : notnull
{
    /// <summary>
    /// Provides the non-generic implementation of message handling by casting the input message
    /// to the strongly-typed message type and delegating to the typed Handle method.
    /// </summary>
    /// <param name="message">The untyped message object to be processed.</param>
    /// <returns>The result of processing the message through the strongly-typed handler.</returns>
    object IMessageMainHandler.Handle(object message)
    {
        return Handle((TMessage)message);
    }

    /// <summary>
    /// Handles a strongly-typed message and returns a strongly-typed result.
    /// This method contains the core business logic for processing the specific message type.
    /// </summary>
    /// <param name="message">The strongly-typed message to be processed by the handler.</param>
    /// <returns>The strongly-typed result of processing the message.</returns>
    TMessageResult Handle(TMessage message);
}
