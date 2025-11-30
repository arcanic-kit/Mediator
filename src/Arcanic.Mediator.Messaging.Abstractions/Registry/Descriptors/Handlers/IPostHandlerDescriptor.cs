namespace Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

/// <summary>
/// Defines a descriptor for post-handlers that process messages after the main handler within the mediator framework.
/// Post-handlers are executed after the main handler completes and are typically used for cross-cutting concerns
/// such as logging, caching, cleanup operations, or other follow-up activities.
/// </summary>
public interface IPostHandlerDescriptor : IHandlerDescriptor {}