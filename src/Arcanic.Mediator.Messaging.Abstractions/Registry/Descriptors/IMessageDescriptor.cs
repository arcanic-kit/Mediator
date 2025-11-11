using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;

/// <summary>
/// Defines a descriptor that contains metadata about a message type and its associated handlers.
/// This interface provides information about message types and manages their main handler registrations.
/// </summary>
public interface IMessageDescriptor
{
    /// <summary>
    /// Gets the type of message that this descriptor represents.
    /// </summary>
    /// <value>The message type associated with this descriptor.</value>
    Type MessageType { get; }

    /// <summary>
    /// Gets a value indicating whether the message type is a generic type definition.
    /// This property helps distinguish between open generic types and closed generic types.
    /// </summary>
    /// <value>True if the message type is generic; otherwise, false.</value>
    bool IsGeneric { get; }

    /// <summary>
    /// Gets a read-only collection of main handler descriptors associated with this message type.
    /// Main handlers are the primary processors for the message type.
    /// </summary>
    /// <value>A read-only collection containing all main handler descriptors for this message.</value>
    IReadOnlyCollection<IMainHandlerDescriptor> MainHandlers { get; }

    /// <summary>
    /// Adds a main handler descriptor to this message descriptor.
    /// This allows registration of additional handlers that can process the message type.
    /// </summary>
    /// <param name="handlerDescriptor">The main handler descriptor to add to this message descriptor.</param>
    void AddMainHandler(IMainHandlerDescriptor handlerDescriptor);
}
