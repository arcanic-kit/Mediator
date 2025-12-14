using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;
using Arcanic.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Messaging.Mediator.Strategies;

/// <summary>
/// Provides a strategy for mediating messages through a complete pipeline including pre-handlers, 
/// a single main handler that returns a result, and post-handlers. This strategy ensures that exactly 
/// one main handler is registered for the message type and executes the complete pipeline in the correct order.
/// Optimized version that caches pipeline behaviors to avoid repeated service resolution.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this strategy produces from message mediation.</typeparam>
public sealed class MessageMediatorRequestPipelineHandlerStrategy<TMessage, TMessageResult> : IMessageMediatorStrategy<TMessage, Task<TMessageResult>> where TMessage : notnull
{
    /// <summary>
    /// The service provider used to resolve pipeline behaviors from the dependency injection container.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Cached pipeline behaviors to avoid repeated service resolution.
    /// </summary>
    private readonly Lazy<IRequestPipelineBehavior<TMessage, TMessageResult>[]> _cachedBehaviors;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageMediatorRequestPipelineHandlerStrategy{TMessage,TMessageResult}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve pipeline behaviors and handlers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
    public MessageMediatorRequestPipelineHandlerStrategy(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        
        // Cache behaviors resolution to avoid repeated service calls
        _cachedBehaviors = new Lazy<IRequestPipelineBehavior<TMessage, TMessageResult>[]>(() =>
        {
            var behaviorType = typeof(IRequestPipelineBehavior<,>).MakeGenericType(typeof(TMessage), typeof(TMessageResult));
            return _serviceProvider.GetServices(behaviorType)
                .Cast<IRequestPipelineBehavior<TMessage, TMessageResult>>()
                .ToArray();
        });
    }

    /// <summary>
    /// Mediates the specified message through the complete pipeline including registered behaviors, 
    /// pre-handlers, main handler, and post-handlers. This method builds a behavior pipeline that 
    /// wraps the core handler execution and ensures proper order of execution.
    /// </summary>
    /// <param name="message">The message to mediate through the pipeline.</param>
    /// <param name="handlerProvider">The provider that supplies the handlers for the message type.</param>
    /// <param name="context">The mediation context containing cancellation tokens and other contextual information.</param>
    /// <returns>A task that represents the asynchronous mediation operation and contains the result from the main handler.</returns>
    /// <remarks>
    /// The mediation follows this execution order:
    /// 1. Pipeline behaviors (in registration order, each wrapping the next)
    /// 2. Pre-handlers (executed in parallel)
    /// 3. Main handler (single handler, executed sequentially)
    /// 4. Post-handlers (executed in parallel)
    /// </remarks>
    public async Task<TMessageResult> Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        var behaviors = _cachedBehaviors.Value;

        // Create the core handler execution delegate
        RequestPipelineDelegate<TMessageResult> handlerExecution = (CancellationToken cancellationToken) =>
        {
            return ExecuteHandlerPipelineAsync(message, handlerProvider, context);
        };

        // Build the behavior pipeline in reverse order (last behavior wraps first)
        // Process in reverse to maintain execution order
        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var currentBehavior = behaviors[i];
            var nextDelegate = handlerExecution;

            handlerExecution = (CancellationToken cancellationToken) =>
            {
                return currentBehavior.HandleAsync(message, nextDelegate, context.CancellationToken);
            };
        }

        return await handlerExecution();
    }

    /// <summary>
    /// Executes the core handler pipeline consisting of pre-handlers, the main handler, and post-handlers.
    /// This method enforces the requirement that exactly one main handler must be registered for the message type.
    /// </summary>
    /// <param name="message">The message to process through the handler pipeline.</param>
    /// <param name="handlerProvider">The provider that supplies the handlers for the message type.</param>
    /// <param name="context">The mediation context containing cancellation tokens and other contextual information.</param>
    /// <returns>A task that represents the asynchronous handler pipeline execution and contains the result from the main handler.</returns>
    /// <exception cref="Exception">
    /// Thrown when:
    /// - More than one main handler is registered for the message type
    /// - No main handler is found for the message type
    /// - An error occurs during pipeline execution
    /// </exception>
    /// <remarks>
    /// The execution order is:
    /// 1. All pre-handlers execute in parallel and must complete before proceeding
    /// 2. The single main handler executes and produces the result
    /// 3. All post-handlers execute in parallel (fire-and-forget style)
    /// </remarks>
    public async Task<TMessageResult> ExecuteHandlerPipelineAsync(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        var mainHandlers = handlerProvider.MainHandlers;
        
        if (mainHandlers.Count > 1)
        {
            throw new Exception("Only one main handler is allowed for command mediation.");
        }

        if (mainHandlers.Count == 0)
        {
            throw new Exception("No main handler found for command mediation.");
        }

        try
        {
            var preHandlers = handlerProvider.PreHandlers;
            var postHandlers = handlerProvider.PostHandlers;

            // Execute pre-handlers in parallel
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

            // Execute the single main handler and capture result
            var mainHandler = mainHandlers.First(); // Use First() instead of Single() for better performance
            var mainResult = (Task<TMessageResult>)mainHandler.Handle(message);
            var result = await mainResult;

            // Execute post-handlers in parallel (fire and forget pattern)
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
        catch (Exception)
        {
            // TODO: Consider implementing proper exception handling strategy
            throw new Exception("Error occurred during pipeline execution.");
        }
    }
}