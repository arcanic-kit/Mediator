using Arcanic.Mediator.Messaging.Abstractions.Registry.Descriptors.Handlers;

namespace Arcanic.Mediator.Messaging.Registry.Descriptors.Handlers;

/// <summary>
/// Provides a concrete implementation of a post-handler descriptor that contains metadata about
/// a post-handler and its associated message type. This descriptor is used to register and manage
/// post-handlers within the mediator framework.
/// </summary>
internal sealed class PostHandlerDescriptor : HandlerDescriptor, IPostHandlerDescriptor
{
}