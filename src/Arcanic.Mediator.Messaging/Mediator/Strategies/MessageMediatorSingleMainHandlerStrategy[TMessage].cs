using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Mediator.Strategies;

/// <summary>
/// Provides a strategy for mediating messages through a single main handler without returning a result.
/// This strategy ensures that exactly one handler is registered for the message type and executes it asynchronously.
/// Throws an exception if multiple handlers are registered for the same message type.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
public sealed class MessageMediatorSingleMainHandlerStrategy<TMessage> : IMessageMediatorStrategy<TMessage, Task> where TMessage : notnull
{
    /// <summary>
    /// Mediates the processing of a message by executing the single registered main handler.
    /// This method validates that exactly one handler is available, executes it, and waits for completion.
    /// </summary>
    /// <param name="message">The message instance to be processed by the single main handler.</param>
    /// <param name="handlerProvider">The provider that supplies access to the registered main handler for the message type.</param>
    /// <param name="context">The mediation context containing cancellation token and result state information.</param>
    /// <returns>A task that represents the completion of the handler execution.</returns>
    /// <exception cref="Exception">Thrown when more than one main handler is registered for the message type.</exception>
    /// <remarks>
    /// This strategy assumes that the handler returns a Task object. Exceptions thrown by the handler
    /// are currently caught and ignored. The cancellation token from the context is not currently used
    /// for cooperative cancellation.
    /// </remarks>
    public async Task Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        if (handlerProvider.MainHandlers.Count > 1)
        {
            throw new Exception();
        }

        Task? messageResult = null;

        try
        {
            var handler = handlerProvider.MainHandlers.Single();

            messageResult = (Task) handler.Handle(message);

            await messageResult;
        }
        catch (Exception) 
        {
            // TODO: Consider implementing proper exception handling strategy
        }
    }
}
