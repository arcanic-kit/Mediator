using Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Messaging.Abstractions.Mediator;

/// <summary>
/// Defines a provider that supplies main handlers for message mediation within the mediator framework.
/// This interface serves as a bridge between the mediator and the collection of registered main handlers,
/// providing access to handlers that can process messages during mediation.
/// </summary>
public interface IMessageMediatorHandlerProvider
{
    /// <summary>
    /// Gets a read-only collection of main handlers available for message processing.
    /// These handlers are the primary processors that execute the business logic for messages
    /// routed through the mediator.
    /// </summary>
    /// <value>A read-only collection containing all registered main handlers for message processing.</value>
    IReadOnlyCollection<IMessageMainHandler> MainHandlers { get; }
}
