using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;
using Arcanic.Mediator.Messaging.Registry.Descriptors;
using Arcanic.Mediator.Messaging.Registry.Descriptors.Handlers;
using System.Collections.Concurrent;
using Arcanic.Mediator.Abstractions.Handler.Main;
using Arcanic.Mediator.Abstractions.Handler.Post;
using Arcanic.Mediator.Abstractions.Handler.Pre;

namespace Arcanic.Mediator.Messaging.Registry;

/// <summary>
/// Provides the concrete implementation of the message registry that manages message types and their associated handlers.
/// This registry maintains thread-safe storage of message descriptors and supports registration of message-handler mappings
/// including main, pre, and post handlers. Optimized for performance with minimal locking.
/// </summary>
public sealed class MessageRegistry : IMessageRegistry
{
    /// <summary>
    /// The internal collection of message descriptors that stores all registered message types and their handlers.
    /// Uses ConcurrentDictionary for thread-safe access without additional locking.
    /// </summary>
    public ConcurrentDictionary<Type, IMessageDescriptor> MessageDescriptors { get; } = new();

    /// <summary>
    /// Registers a message type with its corresponding handler type in the registry.
    /// This method creates or updates the message descriptor to include the specified handler,
    /// handling both generic and non-generic message types appropriately and supporting main, pre, and post handlers.
    /// </summary>
    /// <param name="messageType">The type of message to register. Cannot be null.</param>
    /// <param name="messageHandler">The type of handler that processes the specified message type.</param>
    /// <param name="onlyOneMainHandler">Whether to enforce only one main handler per message type.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageType"/> is null.</exception>
    /// <remarks>
    /// This method is thread-safe using ConcurrentDictionary's atomic operations.
    /// If the message type is generic, it will be normalized to its generic type definition.
    /// Handlers are classified as main, pre, or post handlers based on their implemented interfaces.
    /// </remarks>
    public void Register(Type messageType, Type messageHandler, bool onlyOneMainHandler = true)
    {
        ArgumentNullException.ThrowIfNull(messageType);
        ArgumentNullException.ThrowIfNull(messageHandler);

        // Normalize generic types to their generic type definition
        if (messageType.IsGenericType)
        {
            messageType = messageType.GetGenericTypeDefinition();
        }

        // Use GetOrAdd for atomic get-or-create operation
        var messageDescriptor = MessageDescriptors.GetOrAdd(messageType, 
            static key => new MessageDescriptor(key));

        // Register handler types based on interfaces they implement
        RegisterHandlerByType(messageDescriptor, messageType, messageHandler, onlyOneMainHandler);
    }

    /// <summary>
    /// Registers a handler based on the interfaces it implements.
    /// This method assumes the message descriptor's Add methods are thread-safe.
    /// </summary>
    private static void RegisterHandlerByType(IMessageDescriptor messageDescriptor, Type messageType, Type messageHandler, bool onlyOneMainHandler)
    {
        // Check for main handler interface
        if (messageHandler.IsAssignableTo(typeof(IMessageMainHandler)))
        {
            if (onlyOneMainHandler && messageDescriptor.MainHandlers.Count > 0)
            {
                return;
            }

            var mainHandler = new MainHandlerDescriptor() 
            { 
                MessageType = messageType,
                HandlerType = messageHandler
            };

            messageDescriptor.AddMainHandler(mainHandler);
        }

        // Check for pre-handler interface
        if (messageHandler.IsAssignableTo(typeof(IMessagePreHandler)))
        {
            var preHandler = new PreHandlerDescriptor() 
            { 
                MessageType = messageType,
                HandlerType = messageHandler
            };

            messageDescriptor.AddPreHandler(preHandler);
        }

        // Check for post-handler interface
        if (messageHandler.IsAssignableTo(typeof(IMessagePostHandler)))
        {
            var postHandler = new PostHandlerDescriptor() 
            { 
                MessageType = messageType,
                HandlerType = messageHandler
            };

            messageDescriptor.AddPostHandler(postHandler);
        }
    }
}
