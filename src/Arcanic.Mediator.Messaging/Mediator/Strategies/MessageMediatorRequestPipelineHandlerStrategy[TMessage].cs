using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;
using Arcanic.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Messaging.Mediator.Strategies;

/// <summary>
/// Provides a strategy for mediating messages through a complete pipeline including pre-handlers, 
/// a single main handler, and post-handlers. This strategy ensures that exactly one main handler 
/// is registered for the message type and executes the complete pipeline in the correct order.
/// Optimized version that caches pipeline behaviors to avoid repeated service resolution.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
/// <remarks>
/// This strategy implements the pipeline pattern for message processing, where:
/// <list type="number">
/// <item>Pipeline behaviors are executed in order, wrapping the core handler execution</item>
/// <item>Pre-handlers are executed in parallel before the main handler</item>
/// <item>Exactly one main handler is executed</item>
/// <item>Post-handlers are executed in parallel after the main handler</item>
/// </list>
/// The strategy enforces that only one main handler is registered for each message type to maintain consistency in command handling.
/// </remarks>
public sealed class MessageMediatorRequestPipelineHandlerStrategy<TMessage> : IMessageMediatorStrategy<TMessage, Task> where TMessage : notnull
{
    /// <summary>
    /// The service provider used to resolve pipeline behaviors from the dependency injection container.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Cached pipeline behaviors to avoid repeated service resolution.
    /// </summary>
    private readonly Lazy<IRequestPipelineBehavior<TMessage, Task>[]> _cachedBehaviors;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageMediatorRequestPipelineHandlerStrategy{TMessage}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve pipeline behaviors and handlers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is null.</exception>
    public MessageMediatorRequestPipelineHandlerStrategy(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        
        // Cache behaviors resolution to avoid repeated service calls
        _cachedBehaviors = new Lazy<IRequestPipelineBehavior<TMessage, Task>[]>(() =>
        {
            var behaviorType = typeof(IRequestPipelineBehavior<,>).MakeGenericType(typeof(TMessage), typeof(Task));
            return _serviceProvider.GetServices(behaviorType)
                .Cast<IRequestPipelineBehavior<TMessage, Task>>()
                .ToArray();
        });
    }

    /// <summary>
    /// Mediates the specified message through the complete pipeline including behaviors, pre-handlers, main handler, and post-handlers.
    /// </summary>
    /// <param name="message">The message to be mediated through the pipeline.</param>
    /// <param name="handlerProvider">The provider that contains the registered handlers for the message type.</param>
    /// <param name="context">The context containing information about the mediation request, including cancellation token.</param>
    /// <returns>A task that represents the asynchronous mediation operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters is null.</exception>
    /// <exception cref="Exception">Thrown when no main handler is found or more than one main handler is registered for the message type.</exception>
    /// <remarks>
    /// The mediation process follows this sequence:
    /// <list type="number">
    /// <item>Resolves all registered pipeline behaviors for the message type</item>
    /// <item>Builds a behavior pipeline by wrapping behaviors in reverse order</item>
    /// <item>Executes the behavior pipeline, which in turn executes the handler pipeline</item>
    /// </list>
    /// </remarks>
    public async Task Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
    {
        var behaviors = _cachedBehaviors.Value;

        // Create the core handler execution delegate
        RequestPipelineDelegate<Task> handlerExecution = async (CancellationToken cancellationToken) =>
        {
            await ExecuteHandlerPipelineAsync(message, handlerProvider, context);
            return Task.CompletedTask;
        };

        // Build the behavior pipeline in reverse order (last behavior wraps first)
        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var currentBehavior = behaviors[i];
            var nextDelegate = handlerExecution;

            handlerExecution = async (CancellationToken cancellationToken) =>
            {
                return await currentBehavior.HandleAsync(message, nextDelegate, context.CancellationToken);
            };
        }

        await handlerExecution();
    }

    /// <summary>
    /// Executes the handler pipeline consisting of pre-handlers, a single main handler, and post-handlers.
    /// </summary>
    /// <param name="message">The message to be processed by the handlers.</param>
    /// <param name="handlerProvider">The provider containing the registered handlers for the message type.</param>
    /// <param name="context">The context containing information about the mediation request.</param>
    /// <returns>A task that represents the asynchronous handler pipeline execution.</returns>
    /// <exception cref="Exception">
    /// Thrown in the following scenarios:
    /// <list type="bullet">
    /// <item>When more than one main handler is registered for the message type</item>
    /// <item>When no main handler is found for the message type</item>
    /// <item>When an error occurs during pipeline execution</item>
    /// </list>
    /// </exception>
    /// <remarks>
    /// The execution order is:
    /// <list type="number">
    /// <item>Pre-handlers are executed in parallel</item>
    /// <item>The single main handler is executed</item>
    /// <item>Post-handlers are executed in parallel</item>
    /// </list>
    /// If any step fails, the entire pipeline is considered failed and an exception is thrown.
    /// </remarks>
    public async Task ExecuteHandlerPipelineAsync(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context)
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

            // Execute the single main handler
            var mainHandler = mainHandlers.First(); // Use First() instead of Single() for better performance
            var mainResult = (Task)mainHandler.Handle(message);
            await mainResult;

            // Execute post-handlers in parallel
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
        catch (Exception)
        {
            // TODO: Consider implementing proper exception handling strategy
            throw new Exception("Error occurred during pipeline execution.");
        }
    }
}