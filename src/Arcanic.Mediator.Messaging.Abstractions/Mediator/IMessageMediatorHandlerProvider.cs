using Arcanic.Mediator.Abstractions.Handler.Main;
using Arcanic.Mediator.Abstractions.Handler.Post;
using Arcanic.Mediator.Abstractions.Handler.Pre;

namespace Arcanic.Mediator.Messaging.Abstractions.Mediator;

/// <summary>
/// Defines a provider that supplies main, pre, and post handlers for message mediation within the mediator framework.
/// This interface serves as a bridge between the mediator and the collection of registered handlers,
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

    /// <summary>
    /// Gets a read-only collection of pre-handlers available for message pre-processing.
    /// These handlers execute before the main handler and are typically used for cross-cutting concerns
    /// such as validation, authentication, or logging.
    /// </summary>
    /// <value>A read-only collection containing all registered pre-handlers for message pre-processing.</value>
    IReadOnlyCollection<IMessagePreHandler> PreHandlers { get; }

    /// <summary>
    /// Gets a read-only collection of post-handlers available for message post-processing.
    /// These handlers execute after the main handler and are typically used for follow-up activities
    /// such as logging, caching, or cleanup operations.
    /// </summary>
    /// <value>A read-only collection containing all registered post-handlers for message post-processing.</value>
    IReadOnlyCollection<IMessagePostHandler> PostHandlers { get; }
}
