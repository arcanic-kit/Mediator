using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;

namespace Arcanic.Mediator.Messaging.Abstractions.Registry;

/// <summary>
/// Defines a registry for managing message types and their associated handlers within the mediator framework.
/// The registry maintains descriptors that map messages to their corresponding handler implementations.
/// </summary>
public interface IMessageRegistry
{
    /// <summary>
    /// Gets a read-only collection of all registered message descriptors.
    /// Each descriptor contains information about a message type and its associated handlers.
    /// </summary>
    /// <value>A read-only list containing all message descriptors registered in the registry.</value>
    IReadOnlyList<IMessageDescriptor> MessageDescriptors { get; }

    /// <summary>
    /// Registers a message type with its corresponding handler type in the registry.
    /// This creates or updates the message descriptor to include the specified handler.
    /// </summary>
    /// <param name="messageType">The type of message to register.</param>
    /// <param name="messageHandler">The type of handler that processes the specified message type.</param>
    void Register(Type messageType, Type messageHandler);
}
