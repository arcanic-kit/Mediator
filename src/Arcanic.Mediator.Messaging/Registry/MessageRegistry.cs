using Arcanic.Mediator.Messaging.Abstractions.Handler.Main;
using Arcanic.Mediator.Messaging.Abstractions.Handler.Pre;
using Arcanic.Mediator.Messaging.Abstractions.Handler.Post;
using Arcanic.Mediator.Messaging.Abstractions.Registry;
using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;
using Arcanic.Mediator.Messaging.Registry.Descriptors;
using Arcanic.Mediator.Messaging.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Registry;

/// <summary>
/// Provides the concrete implementation of the message registry that manages message types and their associated handlers.
/// This registry maintains thread-safe storage of message descriptors and supports registration of message-handler mappings
/// including main, pre, and post handlers.
/// </summary>
public class MessageRegistry : IMessageRegistry
{
    /// <summary>
    /// The synchronization object used to ensure thread-safe access to the registry.
    /// </summary>
    private readonly object _lock = new object();
    
    /// <summary>
    /// The internal collection of message descriptors that stores all registered message types and their handlers.
    /// </summary>
    private readonly List<IMessageDescriptor> _messageDescriptors = new();

    /// <summary>
    /// Gets a read-only collection of all registered message descriptors.
    /// Each descriptor contains information about a message type and its associated handlers.
    /// </summary>
    /// <value>A read-only list containing all message descriptors registered in the registry.</value>
    public IReadOnlyList<IMessageDescriptor> MessageDescriptors => _messageDescriptors.AsReadOnly();

    /// <summary>
    /// Registers a message type with its corresponding handler type in the registry.
    /// This method creates or updates the message descriptor to include the specified handler,
    /// handling both generic and non-generic message types appropriately and supporting main, pre, and post handlers.
    /// </summary>
    /// <param name="messageType">The type of message to register. Cannot be null.</param>
    /// <param name="messageHandler">The type of handler that processes the specified message type.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageType"/> is null.</exception>
    /// <remarks>
    /// This method is thread-safe and supports registration of generic type definitions.
    /// If the message type is generic, it will be normalized to its generic type definition.
    /// Handlers are classified as main, pre, or post handlers based on their implemented interfaces.
    /// </remarks>
    public void Register(Type messageType, Type messageHandler)
    {
        ArgumentNullException.ThrowIfNull(messageType);

        lock (_lock)
        {
            if (messageType.IsGenericType)
            {
                messageType = messageType.GetGenericTypeDefinition();
            }

            var messageDescriptor = _messageDescriptors.FirstOrDefault(x => x.MessageType == messageType);
            if (messageDescriptor is null)
            {
                messageDescriptor = new MessageDescriptor(messageType);
                _messageDescriptors.Add(messageDescriptor);
            }

            if (messageHandler.IsAssignableTo(typeof(IMessageMainHandler)))
            {
                var mainHandler = new MainHandlerDescriptor() 
                { 
                    MessageType = messageType,
                    HandlerType = messageHandler
                };

                messageDescriptor.AddMainHandler(mainHandler);
            }

            if (messageHandler.IsAssignableTo(typeof(IMessagePreHandler)))
            {
                var preHandler = new PreHandlerDescriptor() 
                { 
                    MessageType = messageType,
                    HandlerType = messageHandler
                };

                messageDescriptor.AddPreHandler(preHandler);
            }

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
}
