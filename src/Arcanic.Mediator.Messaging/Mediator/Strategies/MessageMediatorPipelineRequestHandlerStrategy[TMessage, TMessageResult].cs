using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Mediator.Strategies;

/// <summary>
/// Provides a strategy for mediating messages through a complete pipeline including pre-handlers, 
/// a single main handler that returns a result, and post-handlers. This strategy ensures that exactly 
/// one main handler is registered for the message type and executes the complete pipeline in the correct order.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this strategy produces from message mediation.</typeparam>
public sealed class MessageMediatorPipelineRequestHandlerStrategy<TMessage, TMessageResult> : IMessageMediatorStrategy<TMessage, Task<TMessageResult>> where TMessage : notnull
{
    /// <summary>
    /// Mediates the processing of a message through the complete pipeline: pre-handlers, main handler, and post-handlers.
    /// This method ensures that all pre-handlers execute before the main handler, and all post-handlers execute after,
    /// while preserving and returning the result from the main handler.
    /// </summary>
    /// <param name="message">The message instance to be processed through the pipeline.</param>
    /// <param name="handlerProvider">The provider that supplies access to all registered handlers for the message type.</param>
    /// <param name="context">The mediation context containing cancellation token and result state information.</param>
    /// <returns>A task that represents the completion of the complete pipeline execution and contains the typed result from the main handler.</returns>
    /// <exception cref="Exception">Thrown when more than one main handler is registered for the message type, or when any handler execution fails.</exception>
    /// <remarks>
    /// This strategy executes handlers in the following order:
    /// 1. All pre-handlers in parallel
    /// 2. Single main handler (returning TMessageResult)
    /// 3. All post-handlers in parallel (with access to the main handler result)
    /// Exceptions thrown during any phase are re-thrown as generic Exception instances.
    /// </remarks>
    public async Task<TMessageResult> Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        if (handlerProvider.MainHandlers.Count > 1)
        {
            throw new Exception("Only one main handler is allowed for command mediation.");
        }

        if (handlerProvider.MainHandlers.Count == 0)
        {
            throw new Exception("No main handler found for command mediation.");
        }

        try
        {
            // Execute pre-handlers in parallel
            if (handlerProvider.PreHandlers.Count > 0)
            {
                var preHandlerTasks = handlerProvider.PreHandlers
                    .Select(preHandler => (Task)preHandler.Handle(message));
                
                await Task.WhenAll(preHandlerTasks);
            }

            // Execute the single main handler and capture result
            var mainHandler = handlerProvider.MainHandlers.Single();
            var mainResult = (Task<TMessageResult>)mainHandler.Handle(message);
            var result = await mainResult;

            // Execute post-handlers in parallel
            if (handlerProvider.PostHandlers.Count > 0)
            {
                var postHandlerTasks = handlerProvider.PostHandlers
                    .Select(postHandler => (Task)postHandler.Handle(message));

                await Task.WhenAll(postHandlerTasks);
            }

            return result;
        }
        catch (Exception)
        {
            // TODO: Consider implementing proper exception handling strategy
            throw new Exception("Error occurred during pipeline execution.");
        }
    }
}