using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Pipeline;

/// <summary>
/// Composes pipeline behaviors around a handler and executes the resulting chain.
/// Shared by the command, query and event dispatchers.
/// </summary>
internal static class PipelineRunner
{
    /// <summary>
    /// Wraps <paramref name="handler"/> with <paramref name="behaviorsInnermostFirst"/> and runs the chain.
    /// </summary>
    /// <param name="message">The message flowing through the pipeline.</param>
    /// <param name="behaviorsInnermostFirst">Behaviors ordered from the one closest to the handler to the outermost one.</param>
    /// <param name="handler">The terminal handler delegate.</param>
    /// <param name="cancellationToken">
    /// The token for the dispatch. A behavior that calls <c>next()</c> without a token gets this token,
    /// so cancellation keeps flowing through the chain.
    /// </param>
    public static Task<TResponse> RunAsync<TMessage, TResponse>(
        TMessage message,
        IEnumerable<IPipelineBehavior<TMessage, TResponse>> behaviorsInnermostFirst,
        PipelineDelegate<TResponse> handler,
        CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        PipelineDelegate<TResponse> next = t => handler(t == default ? cancellationToken : t);

        foreach (var behavior in behaviorsInnermostFirst)
        {
            var inner = next;
            next = t => behavior.HandleAsync(message, inner, t == default ? cancellationToken : t);
        }

        return next(cancellationToken);
    }
}
