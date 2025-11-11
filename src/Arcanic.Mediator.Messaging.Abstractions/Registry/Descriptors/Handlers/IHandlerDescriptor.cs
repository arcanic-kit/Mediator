namespace Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

/// <summary>
/// Defines a base descriptor that contains metadata about a handler and its associated message type.
/// This interface provides the fundamental information needed to identify handler-message relationships.
/// </summary>
public interface IHandlerDescriptor
{
    /// <summary>
    /// Gets the type of message that this handler can process.
    /// </summary>
    /// <value>The message type that is handled by the associated handler.</value>
    Type MessageType { get; }

    /// <summary>
    /// Gets the type of the handler that processes the associated message type.
    /// </summary>
    /// <value>The handler type that implements the message processing logic.</value>
    Type HandlerType { get; }
}
