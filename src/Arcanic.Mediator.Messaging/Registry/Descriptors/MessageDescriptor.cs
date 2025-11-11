using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors;
using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Registry.Descriptors;

/// <summary>
/// Provides a concrete implementation of a message descriptor that contains metadata about a message type
/// and manages its associated main handlers. This class maintains the relationship between message types
/// and their handlers within the mediator framework.
/// </summary>
public class MessageDescriptor : IMessageDescriptor
{
    /// <summary>
    /// The internal collection of main handler descriptors associated with this message type.
    /// </summary>
    private readonly List<IMainHandlerDescriptor> _mainhandlers = new();

    /// <summary>
    /// Gets the type of message that this descriptor represents.
    /// </summary>
    /// <value>The message type associated with this descriptor.</value>
    public Type MessageType { get; }

    /// <summary>
    /// Gets a value indicating whether the message type is a generic type definition.
    /// This property helps distinguish between open generic types and closed generic types.
    /// </summary>
    /// <value>True if the message type is generic; otherwise, false.</value>
    public bool IsGeneric { get; }

    /// <summary>
    /// Gets a read-only collection of main handler descriptors associated with this message type.
    /// Main handlers are the primary processors for the message type.
    /// </summary>
    /// <value>A read-only collection containing all main handler descriptors for this message.</value>
    public IReadOnlyCollection<IMainHandlerDescriptor> MainHandlers => _mainhandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDescriptor"/> class.
    /// </summary>
    /// <param name="messageType">The type of message that this descriptor will represent.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageType"/> is null.</exception>
    public MessageDescriptor(Type messageType)
    {
        MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
        IsGeneric = messageType.IsGenericType;
    }

    /// <summary>
    /// Adds a main handler descriptor to this message descriptor.
    /// This allows registration of additional handlers that can process the message type.
    /// </summary>
    /// <param name="handlerDescriptor">The main handler descriptor to add to this message descriptor.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handlerDescriptor"/> is null.</exception>
    public void AddMainHandler(IMainHandlerDescriptor handlerDescriptor)
    {
        ArgumentNullException.ThrowIfNull(handlerDescriptor);

        _mainhandlers.Add(handlerDescriptor);
    }
}
