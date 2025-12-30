namespace Arcanic.Mediator.Abstractions.Pipeline;

/// <summary>
/// Defines a pipeline behavior that can wrap the execution of message processing with additional logic.
/// Pipeline behaviors allow for implementing cross-cutting concerns such as logging, validation, 
/// caching, performance monitoring, and exception handling in a composable manner.
/// </summary>
/// <typeparam name="TMessage">The type of message being processed. Must be a non-null reference type.</typeparam>
/// <typeparam name="TResponse">The type of response expected from the message processing.</typeparam>
public interface IPipelineBehavior<in TMessage, TResponse> where TMessage : IMessage
{
    /// <summary>
    /// Handles the message processing by optionally performing pre-processing logic, 
    /// delegating to the next behavior in the pipeline, and optionally performing post-processing logic.
    /// </summary>
    /// <param name="message">The message instance being processed through the pipeline.</param>
    /// <param name="next">The delegate representing the next behavior in the pipeline or the final handler execution.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The result of processing the message through this behavior and the remainder of the pipeline.</returns>
    /// <remarks>
    /// Implementations should typically follow this pattern:
    /// 1. Perform any pre-processing logic
    /// 2. Call await next() to continue the pipeline
    /// 3. Perform any post-processing logic on the result
    /// 4. Return the (potentially modified) result
    /// 
    /// The behavior can choose to:
    /// - Short-circuit the pipeline by not calling next() and returning early
    /// - Modify the message before passing it to next()
    /// - Modify the result after next() returns
    /// - Handle exceptions thrown by next() or downstream behaviors
    /// </remarks>
    Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default);
}