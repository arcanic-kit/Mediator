namespace Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

/// <summary>
/// Defines a descriptor for main handlers that process messages within the mediator framework.
/// Main handlers are the primary processors responsible for handling specific message types,
/// inheriting the base handler descriptor functionality while serving as the primary execution path.
/// </summary>
public interface IMainHandlerDescriptor : IHandlerDescriptor {}
