namespace Arcanic.Mediator.Abstractions.Handler.Main;

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