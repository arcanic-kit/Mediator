using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Registry.Descriptors.Handlers;

/// <summary>
/// Provides an abstract base implementation of a handler descriptor that contains metadata about
/// a handler and its associated message type. This class serves as the foundation for concrete
/// handler descriptor implementations within the mediator framework.
/// </summary>
internal abstract class HandlerDescriptor : IHandlerDescriptor
{
    /// <summary>
    /// Gets or initializes the type of message that this handler can process.
    /// This property is required and must be set during object initialization.
    /// </summary>
    /// <value>The message type that is handled by the associated handler.</value>
    public required Type MessageType { get; init; }

    /// <summary>
    /// Gets or initializes the type of the handler that processes the associated message type.
    /// This property is required and must be set during object initialization.
    /// </summary>
    /// <value>The handler type that implements the message processing logic.</value>
    public required Type HandlerType { get; init; }
}
