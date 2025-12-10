using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;
using Arcanic.Mediator.Messaging.Abstractions.Registry;

namespace Arcanic.Mediator.Messaging.Mediator;

/// <summary>
/// Provides the core implementation of the message mediator that coordinates the processing of messages
/// through registered handlers. This mediator serves as the central hub for message routing, using
/// configurable strategies to determine how messages are processed and handled within the application.
/// High-performance version with optimized hot paths.
/// </summary>
public sealed class MessageMediator : IMessageMediator
{
    /// <summary>
    /// The message registry that contains all registered message types and their associated handlers.
    /// </summary>
    private readonly IMessageRegistry _messageRegistry;
    
    /// <summary>
    /// The service provider used to resolve handler instances from the dependency injection container.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageMediator"/> class.
    /// </summary>
    /// <param name="messageRegistry">The message registry containing registered message types and handlers.</param>
    /// <param name="serviceProvider">The service provider used to resolve handler instances from the DI container.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageRegistry"/> or <paramref name="serviceProvider"/> is null.</exception>
    public MessageMediator(IMessageRegistry messageRegistry,
                           IServiceProvider serviceProvider)
    {
        _messageRegistry = messageRegistry ?? throw new ArgumentNullException(nameof(messageRegistry));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Mediates the processing of a message using the specified options and strategy.
    /// The mediator coordinates between the message, its handlers, and the execution strategy
    /// to produce the desired result. This method handles both generic and non-generic message types
    /// and manages the execution context throughout the mediation process.
    /// High-performance version with caching optimizations.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to mediate. Must be a non-null reference type.</typeparam>
    /// <typeparam name="TMessageResult">The type of result expected from processing the message.</typeparam>
    /// <param name="message">The message instance to be processed through the mediator.</param>
    /// <param name="options">The mediation options that specify the strategy and cancellation token for processing.</param>
    /// <returns>The result of processing the message through the configured strategy and handlers.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no message descriptor is registered for the message type.</exception>
    /// <remarks>
    /// This method creates a mediation context with the provided cancellation token and manages its lifecycle
    /// using a disposable scope. For generic message types, the method normalizes them to their generic type
    /// definitions for lookup in the registry. The actual message type is passed to the handler provider
    /// to ensure proper generic type construction for handlers.
    /// </remarks>
    public TMessageResult Mediate<TMessage, TMessageResult>(TMessage message, IMessageMediatorOptions<TMessage, TMessageResult> options) where TMessage : notnull
    {
        var messageExecutionContext = new MessageMediatorContext(options.CancellationToken);

        // Use a scope to manage the execution context
        using var _ = MessageMediatorContextAccessor.CreateScope(messageExecutionContext);

        // Get the actual type of the message
        var messageType = message.GetType();
        var lookupType = messageType.IsGenericType ? messageType.GetGenericTypeDefinition() : messageType;

        _messageRegistry.MessageDescriptors.TryGetValue(lookupType, out var messageDescriptor);

        if (messageDescriptor == null)
            throw new InvalidOperationException($"No message descriptor registered for message type {lookupType}");

        var messageMediatorHandlerProvider = new MessageMediatorHandlerProvider(messageType, messageDescriptor, _serviceProvider);

        return options.Strategy.Mediate(message, messageMediatorHandlerProvider, MessageMediatorContextAccessor.Current);
    }
}
