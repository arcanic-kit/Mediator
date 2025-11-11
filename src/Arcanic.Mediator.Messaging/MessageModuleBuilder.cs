using Arcanic.Mediator.Messaging.Abstractions.Registry;

namespace Arcanic.Mediator.Messaging;

/// <summary>
/// Provides a fluent builder for registering message types and their handlers with the message registry.
/// This builder simplifies the process of registering multiple message-handler pairs in a chainable manner.
/// </summary>
public sealed class MessageModuleBuilder
{
    /// <summary>
    /// The message registry used to store message type and handler type registrations.
    /// </summary>
    private readonly IMessageRegistry _messageRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageModuleBuilder"/> class.
    /// </summary>
    /// <param name="messageRegistry">The message registry to use for storing message and handler registrations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageRegistry"/> is null.</exception>
    public MessageModuleBuilder(IMessageRegistry messageRegistry)
    {
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
    }

    /// <summary>
    /// Registers a message type with its corresponding handler type in the message registry.
    /// This method supports fluent chaining to allow multiple registrations in a single expression.
    /// </summary>
    /// <param name="messageType">The type of message to register with the registry.</param>
    /// <param name="handlerType">The type of handler that processes the specified message type.</param>
    /// <returns>The current <see cref="MessageModuleBuilder"/> instance to support fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageType"/> or <paramref name="handlerType"/> is null.</exception>
    public MessageModuleBuilder Register(Type messageType, Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(messageType);
        ArgumentNullException.ThrowIfNull(handlerType);

        _messageRegistry.Register(messageType, handlerType);

        return this;
    }
}
