using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Registry.Descriptors.Handlers;

/// <summary>
/// Provides a concrete implementation of a pre-handler descriptor that contains metadata about
/// a pre-handler and its associated message type. This descriptor is used to register and manage
/// pre-handlers within the mediator framework.
/// </summary>
internal sealed class PreHandlerDescriptor : HandlerDescriptor, IPreHandlerDescriptor
{
}