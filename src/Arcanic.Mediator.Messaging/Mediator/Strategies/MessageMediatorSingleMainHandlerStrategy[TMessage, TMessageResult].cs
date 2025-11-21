using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Mediator.Strategies;

/// <summary>
/// Provides a strategy for mediating messages through a single main handler that returns a typed result.
/// This strategy ensures that exactly one handler is registered for the message type and executes it asynchronously,
/// returning the handler's result. Throws an exception if multiple handlers are registered for the same message type.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this strategy produces from message mediation.</typeparam>
public sealed class MessageMediatorSingleMainHandlerStrategy<TMessage, TMessageResult> : IMessageMediatorStrategy<TMessage, Task<TMessageResult>> where TMessage : notnull
{
    /// <summary>
    /// Mediates the processing of a message by executing the single registered main handler and returning its result.
    /// This method validates that exactly one handler is available, executes it, and returns the typed result.
    /// </summary>
    /// <param name="message">The message instance to be processed by the single main handler.</param>
    /// <param name="handlerProvider">The provider that supplies access to the registered main handler for the message type.</param>
    /// <param name="context">The mediation context containing cancellation token and result state information.</param>
    /// <returns>A task that represents the completion of the handler execution and contains the typed result.</returns>
    /// <exception cref="Exception">Thrown when more than one main handler is registered for the message type, or when the handler execution fails.</exception>
    /// <remarks>
    /// This strategy assumes that the handler returns a Task&lt;TMessageResult&gt; object. Any exceptions thrown
    /// during handler execution are re-thrown as generic Exception instances. The cancellation token from the
    /// context is not currently used for cooperative cancellation.
    /// </remarks>
    public async Task<TMessageResult> Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        if (handlerProvider.MainHandlers.Count > 1)
        {
            throw new Exception();
        }

        Task<TMessageResult>? messageResult = null;

        try
        {
            var handler = handlerProvider.MainHandlers.Single();

            messageResult = (Task<TMessageResult>)handler.Handle(message);

            return await messageResult;
        }
        catch (Exception)
        {
            // TODO: Consider implementing proper exception handling strategy
            throw new Exception();
        }
    }
}