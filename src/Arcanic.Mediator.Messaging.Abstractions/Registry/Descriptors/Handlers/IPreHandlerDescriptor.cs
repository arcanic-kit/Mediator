namespace Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

/// <summary>
/// Defines a descriptor for pre-handlers that process messages before the main handler within the mediator framework.
/// Pre-handlers are executed prior to the main handler and are typically used for cross-cutting concerns
/// such as validation, authentication, or logging.
/// </summary>
public interface IPreHandlerDescriptor : IHandlerDescriptor {}