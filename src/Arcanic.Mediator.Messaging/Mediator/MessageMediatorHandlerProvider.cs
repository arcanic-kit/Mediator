using Arcanic.Mediator.Abstractions.Handler.Main;
using Arcanic.Mediator.Abstractions.Handler.Post;
using Arcanic.Mediator.Abstractions.Handler.Pre;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;
using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Messaging.Mediator;

/// <summary>
/// Provides handler resolution services for message mediation by managing the lifecycle and instantiation
/// of message handlers. This provider resolves main, pre, and post handlers from the dependency injection container 
/// and handles generic type construction for generic message handlers.
/// Optimized version that caches resolved handlers to avoid repeated DI resolution.
/// </summary>
internal sealed class MessageMediatorHandlerProvider : IMessageMediatorHandlerProvider
{
    /// <summary>
    /// Cached main handlers to avoid repeated resolution from DI container.
    /// </summary>
    private readonly Lazy<IReadOnlyCollection<IMessageMainHandler>> _mainHandlers;
    
    /// <summary>
    /// Cached pre-handlers to avoid repeated resolution from DI container.
    /// </summary>
    private readonly Lazy<IReadOnlyCollection<IMessagePreHandler>> _preHandlers;
    
    /// <summary>
    /// Cached post-handlers to avoid repeated resolution from DI container.
    /// </summary>
    private readonly Lazy<IReadOnlyCollection<IMessagePostHandler>> _postHandlers;

    /// <summary>
    /// Gets a read-only collection of main handlers available for message processing.
    /// These handlers are resolved once and cached for subsequent access.
    /// </summary>
    public IReadOnlyCollection<IMessageMainHandler> MainHandlers => _mainHandlers.Value;

    /// <summary>
    /// Gets a read-only collection of pre-handlers available for message pre-processing.
    /// These handlers are resolved once and cached for subsequent access.
    /// </summary>
    public IReadOnlyCollection<IMessagePreHandler> PreHandlers => _preHandlers.Value;

    /// <summary>
    /// Gets a read-only collection of post-handlers available for message post-processing.
    /// These handlers are resolved once and cached for subsequent access.
    /// </summary>
    public IReadOnlyCollection<IMessagePostHandler> PostHandlers => _postHandlers.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageMediatorHandlerProvider"/> class.
    /// </summary>
    /// <param name="messageType">The type of message for which handlers will be resolved.</param>
    /// <param name="messageDescriptor">The descriptor containing handler registration information for the message type.</param>
    /// <param name="serviceProvider">The service provider used to resolve handler instances from the DI container.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters is null.</exception>
    public MessageMediatorHandlerProvider(Type messageType, IMessageDescriptor messageDescriptor, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(messageType);
        ArgumentNullException.ThrowIfNull(messageDescriptor);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        // Initialize lazy handlers to avoid immediate resolution and cache results
        _mainHandlers = new Lazy<IReadOnlyCollection<IMessageMainHandler>>(() =>
            ResolveHandlers(
                messageDescriptor.MainHandlers,
                handlerType => (IMessageMainHandler)serviceProvider.GetRequiredService(handlerType),
                messageType));

        _preHandlers = new Lazy<IReadOnlyCollection<IMessagePreHandler>>(() =>
            ResolveHandlers(
                messageDescriptor.PreHandlers,
                handlerType => (IMessagePreHandler)serviceProvider.GetRequiredService(handlerType),
                messageType));

        _postHandlers = new Lazy<IReadOnlyCollection<IMessagePostHandler>>(() =>
            ResolveHandlers(
                messageDescriptor.PostHandlers,
                handlerType => (IMessagePostHandler)serviceProvider.GetRequiredService(handlerType),
                messageType));
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
    /// <param name="messageType">The message type for generic type construction.</param>
    /// <returns>A read-only collection of resolved handler instances.</returns>
    private static IReadOnlyCollection<THandler> ResolveHandlers<THandler, THandlerDescriptor>(
        IEnumerable<THandlerDescriptor> handlerDescriptors,
        Func<Type, THandler> resolveFunc,
        Type messageType
    ) where THandlerDescriptor : IHandlerDescriptor
    {
        // Pre-size the list if possible to avoid reallocations
        var descriptorList = handlerDescriptors as IList<THandlerDescriptor> ?? handlerDescriptors.ToArray();
        var handlers = new List<THandler>(descriptorList.Count);
        
        foreach (var descriptor in descriptorList)
        {
            var handlerType = GetHandlerType(descriptor.HandlerType, messageType);
            var handler = resolveFunc(handlerType);
            handlers.Add(handler);
        }
        
        return handlers.AsReadOnly();
    }

    /// <summary>
    /// Determines the concrete handler type from a handler descriptor, handling generic type construction
    /// when necessary. For generic handler types, this method constructs the closed generic type using
    /// the generic arguments from the message type.
    /// </summary>
    /// <param name="descriptorType">The handler type from the descriptor, which may be an open generic type.</param>
    /// <param name="messageType">The message type for generic argument construction.</param>
    /// <returns>The concrete handler type that can be instantiated, with generic arguments applied if necessary.</returns>
    private static Type GetHandlerType(Type descriptorType, Type messageType)
    {
        if (!descriptorType.IsGenericType)
        {
            return descriptorType;
        }

        var genericArguments = messageType.GetGenericArguments();
        return descriptorType.MakeGenericType(genericArguments);
    }
}
