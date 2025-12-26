namespace Arcanic.Mediator.Abstractions.Handler.Pre;

/// <summary>
/// Defines a base interface for pre-message handlers that can process messages of any type before the main handler.
/// This interface provides a non-generic entry point for pre-message handling within the mediator framework.
/// </summary>
public interface IMessagePreHandler
{
    /// <summary>
    /// Handles a message before the main handler and returns the processing result.
    /// This method accepts an untyped message object and returns an untyped result.
    /// </summary>
    /// <param name="message">The message object to be pre-processed by the handler.</param>
    /// <returns>The result of pre-processing the message.</returns>
    object Handle(object message);
}