using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Event.Abstractions.Pipeline;

/// <summary>
/// Defines a pipeline behavior for event handling in the mediator pattern.
/// Implementations can add cross-cutting concerns (such as logging, validation, etc.)
/// to the event processing pipeline.
/// </summary>
/// <typeparam name="TEvent">
/// The type of the event being handled. Must implement <see cref="IEvent"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the event handler.
/// </typeparam>
public interface IEventPipelineBehavior<in TEvent, TResponse> : IPipelineBehavior<TEvent, TResponse> where TEvent : IEvent { }