using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;

/// <summary>
/// Defines a descriptor that contains metadata about a message type and its associated handlers.
/// This interface provides information about message types and manages their main, pre, and post handler registrations.
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
    /// Gets a read-only collection of pre-handler descriptors associated with this message type.
    /// Pre-handlers are executed before the main handler for cross-cutting concerns.
    /// </summary>
    /// <value>A read-only collection containing all pre-handler descriptors for this message.</value>
    IReadOnlyCollection<IPreHandlerDescriptor> PreHandlers { get; }

    /// <summary>
    /// Gets a read-only collection of post-handler descriptors associated with this message type.
    /// Post-handlers are executed after the main handler for follow-up activities.
    /// </summary>
    /// <value>A read-only collection containing all post-handler descriptors for this message.</value>
    IReadOnlyCollection<IPostHandlerDescriptor> PostHandlers { get; }

    /// <summary>
    /// Adds a main handler descriptor to this message descriptor.
    /// This allows registration of additional handlers that can process the message type.
    /// </summary>
    /// <param name="handlerDescriptor">The main handler descriptor to add to this message descriptor.</param>
    void AddMainHandler(IMainHandlerDescriptor handlerDescriptor);

    /// <summary>
    /// Adds a pre-handler descriptor to this message descriptor.
    /// This allows registration of pre-handlers that execute before the main handler.
    /// </summary>
    /// <param name="handlerDescriptor">The pre-handler descriptor to add to this message descriptor.</param>
    void AddPreHandler(IPreHandlerDescriptor handlerDescriptor);

    /// <summary>
    /// Adds a post-handler descriptor to this message descriptor.
    /// This allows registration of post-handlers that execute after the main handler.
    /// </summary>
    /// <param name="handlerDescriptor">The post-handler descriptor to add to this message descriptor.</param>
    void AddPostHandler(IPostHandlerDescriptor handlerDescriptor);
}
