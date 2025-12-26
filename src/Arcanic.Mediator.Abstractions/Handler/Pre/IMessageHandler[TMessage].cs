namespace Arcanic.Mediator.Abstractions.Handler.Pre;

/// <summary>
/// Defines a strongly-typed pre-message handler that processes specific message types and returns specific result types.
/// This interface extends the base pre-handler interface while providing type safety for pre-message processing.
/// </summary>
/// <typeparam name="TMessage">The type of message that this pre-handler can process. Must be a non-null reference type.</typeparam>
public interface IMessageHandler<in TMessage> : IMessagePreHandler where TMessage : notnull
{
    /// <summary>
    /// Provides the non-generic implementation of pre-message handling by casting the input message
    /// to the strongly-typed message type and delegating to the typed Handle method.
    /// </summary>
    /// <param name="message">The untyped message object to be pre-processed.</param>
    /// <returns>The result of pre-processing the message through the strongly-typed pre-handler.</returns>
    object IMessagePreHandler.Handle(object message)
    {
        return Handle((TMessage)message);
    }

    /// <summary>
    /// Handles a strongly-typed message before the main handler and returns a strongly-typed result.
    /// This method contains the pre-processing logic for the specific message type.
    /// </summary>
    /// <param name="message">The strongly-typed message to be pre-processed by the handler.</param>
    /// <returns>The strongly-typed result of pre-processing the message.</returns>
    object Handle(TMessage message);
}