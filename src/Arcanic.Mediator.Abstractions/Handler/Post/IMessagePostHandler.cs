namespace Arcanic.Mediator.Abstractions.Handler.Post;

/// <summary>
/// Defines a base interface for post-message handlers that can process messages of any type after the main handler.
/// This interface provides a non-generic entry point for post-message handling within the mediator framework.
/// </summary>
public interface IMessagePostHandler
{
    /// <summary>
    /// Handles a message after the main handler and returns the processing result.
    /// This method accepts an untyped message object and returns an untyped result.
    /// </summary>
    /// <param name="message">The message object to be post-processed by the handler.</param>
    /// <returns>The result of post-processing the message.</returns>
    object Handle(object message);
}