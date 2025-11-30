using Arcanic.Mediator.Messaging.Abstractions.Handler.Main;
using Arcanic.Mediator.Messaging.Abstractions.Handler.Pre;
using Arcanic.Mediator.Messaging.Abstractions.Handler.Post;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;
using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Messaging.Mediator;

/// <summary>
/// Provides handler resolution services for message mediation by managing the lifecycle and instantiation
/// of message handlers. This provider resolves main, pre, and post handlers from the dependency injection container 
/// and handles generic type construction for generic message handlers.
/// </summary>
internal sealed class MessageMediatorHandlerProvider : IMessageMediatorHandlerProvider
{
    /// <summary>
    /// The message type for which handlers are being resolved.
    /// </summary>
    private readonly Type _messageType;
    
    /// <summary>
    /// The service provider used to resolve handler instances from the dependency injection container.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// The message descriptor containing metadata about the message type and its registered handlers.
    /// </summary>
    private readonly IMessageDescriptor _messageDescriptor;

    /// <summary>
    /// Gets a read-only collection of main handlers available for message processing.
    /// These handlers are resolved from the dependency injection container based on the registered
    /// handler descriptors for the current message type.
    /// </summary>
    /// <value>A read-only collection containing all resolved main handlers for message processing.</value>
    public IReadOnlyCollection<IMessageMainHandler> MainHandlers { 
        get {
            return ResolveHandlers(
                _messageDescriptor.MainHandlers,
                handlerType => (IMessageMainHandler) _serviceProvider.GetRequiredService(handlerType)
            );
        } 
    }

    /// <summary>
    /// Gets a read-only collection of pre-handlers available for message pre-processing.
    /// These handlers are resolved from the dependency injection container and execute before the main handler.
    /// </summary>
    /// <value>A read-only collection containing all resolved pre-handlers for message pre-processing.</value>
    public IReadOnlyCollection<IMessagePreHandler> PreHandlers { 
        get {
            return ResolveHandlers(
                _messageDescriptor.PreHandlers,
                handlerType => (IMessagePreHandler) _serviceProvider.GetRequiredService(handlerType)
            );
        } 
    }

    /// <summary>
    /// Gets a read-only collection of post-handlers available for message post-processing.
    /// These handlers are resolved from the dependency injection container and execute after the main handler.
    /// </summary>
    /// <value>A read-only collection containing all resolved post-handlers for message post-processing.</value>
    public IReadOnlyCollection<IMessagePostHandler> PostHandlers { 
        get {
            return ResolveHandlers(
                _messageDescriptor.PostHandlers,
                handlerType => (IMessagePostHandler) _serviceProvider.GetRequiredService(handlerType)
            );
        } 
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageMediatorHandlerProvider"/> class.
    /// </summary>
    /// <param name="messageType">The type of message for which handlers will be resolved.</param>
    /// <param name="messageDescriptor">The descriptor containing handler registration information for the message type.</param>
    /// <param name="serviceProvider">The service provider used to resolve handler instances from the DI container.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters is null.</exception>
    public MessageMediatorHandlerProvider(Type messageType, IMessageDescriptor messageDescriptor, IServiceProvider serviceProvider)
    {
        _messageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
        _messageDescriptor = messageDescriptor ?? throw new ArgumentNullException(nameof(messageDescriptor));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Resolves a collection of handlers from their descriptors using the provided resolution function.
    /// This method processes each handler descriptor to determine the correct handler type and then
    /// uses the resolution function to instantiate the handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of handler being resolved.</typeparam>
    /// <typeparam name="THandlerDescriptor">The type of handler descriptor being processed.</typeparam>
    /// <param name="handlerDescriptors">The collection of handler descriptors to resolve.</param>
    /// <param name="resolveFunc">The function used to resolve handler instances from their types.</param>
    /// <returns>A read-only collection of resolved handler instances.</returns>
    private IReadOnlyCollection<THandler> ResolveHandlers<THandler, THandlerDescriptor>(
        IEnumerable<THandlerDescriptor> handlerDescriptors,
        Func<Type, THandler> resolveFunc
    ) where THandlerDescriptor : IHandlerDescriptor
    {
        return handlerDescriptors
            .Select(descriptor => resolveFunc(GetHandlerType(descriptor.HandlerType)))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Determines the concrete handler type from a handler descriptor, handling generic type construction
    /// when necessary. For generic handler types, this method constructs the closed generic type using
    /// the generic arguments from the message type.
    /// </summary>
    /// <param name="descriptor">The handler type from the descriptor, which may be an open generic type.</param>
    /// <returns>The concrete handler type that can be instantiated, with generic arguments applied if necessary.</returns>
    private Type GetHandlerType(Type descriptor)
    {
        var handlerType = descriptor;

        if (descriptor.IsGenericType)
        {
            handlerType = handlerType.MakeGenericType(_messageType.GetGenericArguments());
        }

        return handlerType;
    }
}
