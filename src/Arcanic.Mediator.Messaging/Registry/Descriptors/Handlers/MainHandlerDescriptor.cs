using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Registry.Descriptors.Handlers;

/// <summary>
/// Provides a concrete implementation of a main handler descriptor that contains metadata about
/// a main handler and its associated message type. This sealed class represents the primary
/// handlers responsible for processing specific message types within the mediator framework.
/// </summary>
internal sealed class MainHandlerDescriptor : HandlerDescriptor, IMainHandlerDescriptor {}