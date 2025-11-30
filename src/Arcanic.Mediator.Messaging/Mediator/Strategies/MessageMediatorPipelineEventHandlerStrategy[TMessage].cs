using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Mediator.Strategies;

/// <summary>
/// Provides a strategy for mediating messages through multiple main handlers concurrently.
/// This strategy executes all registered main handlers for a message type in parallel and waits
/// for all handlers to complete before returning. This is useful for scenarios where multiple
/// independent operations need to be performed for a single message.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
public class MessageMediatorPipelineEventHandlerStrategy<TMessage> : IMessageMediatorStrategy<TMessage, Task> where TMessage : notnull
{
    /// <summary>
    /// Mediates the processing of a message by executing all available main handlers concurrently.
    /// This method collects all handlers from the provider, invokes them in parallel, and waits
    /// for all operations to complete. If any handler throws an exception, the operation continues
    /// with remaining handlers but the exception is silently caught.
    /// </summary>
    /// <param name="message">The message instance to be processed by all main handlers.</param>
    /// <param name="handlerProvider">The provider that supplies access to all registered main handlers for the message type.</param>
    /// <param name="context">The mediation context containing cancellation token and result state information.</param>
    /// <returns>A task that represents the completion of all handler executions.</returns>
    /// <remarks>
    /// This strategy assumes that all handlers return Task objects. Exceptions thrown by individual
    /// handlers are currently caught and ignored, allowing other handlers to continue processing.
    /// The cancellation token from the context is not currently used for cooperative cancellation.
    /// </remarks>
    public async Task Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        List<Task>? messageResults = new();

        try
        {
            if (handlerProvider.PreHandlers.Count > 0)
            {
                var preHandlerTasks = handlerProvider.PreHandlers
                    .Select(preHandler => (Task)preHandler.Handle(message));

                await Task.WhenAll(preHandlerTasks);
            }

            foreach (var handler in handlerProvider.MainHandlers)
            {
                messageResults.Add((Task)handler.Handle(message));
            }

            await Task.WhenAll(messageResults);

            if (handlerProvider.PostHandlers.Count > 0)
            {
                var postHandlerTasks = handlerProvider.PostHandlers
                    .Select(postHandler => (Task)postHandler.Handle(message));

                await Task.WhenAll(postHandlerTasks);
            }
        }
        catch (Exception)
        {
            // TODO: Consider implementing proper exception handling strategy
            // Current implementation silently catches exceptions which may not be desired
        }
    }
}