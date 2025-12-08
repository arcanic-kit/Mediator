using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;
using System.Collections.Concurrent;

namespace Arcanic.Mediator.Messaging.Mediator.Strategies;

/// <summary>
/// A high-performance strategy for simple message mediation that bypasses pipeline behaviors
/// and directly executes handlers. This strategy is optimized for scenarios where no pipeline
/// behaviors are registered and maximum performance is required.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this strategy produces from message mediation.</typeparam>
public sealed class MessageMediatorDirectHandlerStrategy<TMessage, TMessageResult> : IMessageMediatorStrategy<TMessage, Task<TMessageResult>> where TMessage : notnull
{
    /// <summary>
    /// Mediates the specified message directly to the main handler, bypassing pipeline behaviors.
    /// This provides maximum performance for simple scenarios.
    /// </summary>
    /// <param name="message">The message to mediate.</param>
    /// <param name="handlerProvider">The provider that supplies the handlers for the message type.</param>
    /// <param name="context">The mediation context containing cancellation tokens and other contextual information.</param>
    /// <returns>A task that represents the asynchronous mediation operation and contains the result from the main handler.</returns>
    public async Task<TMessageResult> Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        var mainHandlers = handlerProvider.MainHandlers;
        
        if (mainHandlers.Count > 1)
        {
            throw new Exception("Only one main handler is allowed for direct mediation.");
        }

        if (mainHandlers.Count == 0)
        {
            throw new Exception("No main handler found for direct mediation.");
        }

        var preHandlers = handlerProvider.PreHandlers;
        var postHandlers = handlerProvider.PostHandlers;

        // Execute pre-handlers in parallel if any exist
        if (preHandlers.Count > 0)
        {
            var preHandlerTasks = new Task[preHandlers.Count];
            int index = 0;
            foreach (var preHandler in preHandlers)
            {
                preHandlerTasks[index++] = (Task)preHandler.Handle(message);
            }
            await Task.WhenAll(preHandlerTasks);
        }

        // Execute the main handler
        var mainHandler = mainHandlers.First();
        var result = await (Task<TMessageResult>)mainHandler.Handle(message);

        // Execute post-handlers in parallel if any exist (fire and forget)
        if (postHandlers.Count > 0)
        {
            var postHandlerTasks = new Task[postHandlers.Count];
            int index = 0;
            foreach (var postHandler in postHandlers)
            {
                postHandlerTasks[index++] = (Task)postHandler.Handle(message);
            }
            await Task.WhenAll(postHandlerTasks);
        }

        return result;
    }
}

/// <summary>
/// A high-performance strategy for simple void message mediation that bypasses pipeline behaviors
/// and directly executes handlers.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
public sealed class MessageMediatorDirectHandlerStrategy<TMessage> : IMessageMediatorStrategy<TMessage, Task> where TMessage : notnull
{
    /// <summary>
    /// Mediates the specified message directly to the main handler, bypassing pipeline behaviors.
    /// </summary>
    /// <param name="message">The message to mediate.</param>
    /// <param name="handlerProvider">The provider that supplies the handlers for the message type.</param>
    /// <param name="context">The mediation context containing cancellation tokens and other contextual information.</param>
    /// <returns>A task that represents the asynchronous mediation operation.</returns>
    public async Task Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        var mainHandlers = handlerProvider.MainHandlers;
        
        if (mainHandlers.Count > 1)
        {
            throw new Exception("Only one main handler is allowed for direct mediation.");
        }

        if (mainHandlers.Count == 0)
        {
            throw new Exception("No main handler found for direct mediation.");
        }

        var preHandlers = handlerProvider.PreHandlers;
        var postHandlers = handlerProvider.PostHandlers;

        // Execute pre-handlers in parallel if any exist
        if (preHandlers.Count > 0)
        {
            var preHandlerTasks = new Task[preHandlers.Count];
            int index = 0;
            foreach (var preHandler in preHandlers)
            {
                preHandlerTasks[index++] = (Task)preHandler.Handle(message);
            }
            await Task.WhenAll(preHandlerTasks);
        }

        // Execute the main handler
        var mainHandler = mainHandlers.First();
        await (Task)mainHandler.Handle(message);

        // Execute post-handlers in parallel if any exist
        if (postHandlers.Count > 0)
        {
            var postHandlerTasks = new Task[postHandlers.Count];
            int index = 0;
            foreach (var postHandler in postHandlers)
            {
                postHandlerTasks[index++] = (Task)postHandler.Handle(message);
            }
            await Task.WhenAll(postHandlerTasks);
        }
    }
}