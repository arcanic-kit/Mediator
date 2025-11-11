namespace Arcanic.Mediator.Messaging.Abstractions.Mediator;

/// <summary>
/// Defines a mediator that coordinates the processing of messages through registered handlers.
/// The mediator acts as a central hub for message routing, using configurable strategies to determine
/// how messages are processed and handled within the application.
/// </summary>
public interface IMessageMediator
{
    /// <summary>
    /// Mediates the processing of a message using the specified options and strategy.
    /// The mediator coordinates between the message, its handlers, and the execution strategy
    /// to produce the desired result.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to mediate. Must be a non-null reference type.</typeparam>
    /// <typeparam name="TMessageResult">The type of result expected from processing the message.</typeparam>
    /// <param name="message">The message instance to be processed through the mediator.</param>
    /// <param name="options">The mediation options that specify the strategy and cancellation token for processing.</param>
    /// <returns>The result of processing the message through the configured strategy and handlers.</returns>
    TMessageResult Mediate<TMessage, TMessageResult>(TMessage message, IMessageMediatorOptions<TMessage, TMessageResult> options) where TMessage : notnull;
}
