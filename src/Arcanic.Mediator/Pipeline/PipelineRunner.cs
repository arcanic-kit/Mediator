using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Pipeline;

/// <summary>
/// Composes pipeline behaviors around a handler and executes the resulting chain.
/// Shared by the command, query and event dispatchers.
/// </summary>
/// <remarks>
/// Behaviors are supplied as layers, innermost layer first (for example: command behaviors, then request
/// behaviors, then generic behaviors). Within each layer the first registered behavior runs outermost.
/// The chain is built by indexing the resolved arrays backwards, avoiding LINQ iterators on every dispatch.
/// </remarks>
internal static class PipelineRunner
{
    /// <summary>
    /// Wraps <paramref name="handler"/> with the behaviors of each layer and runs the chain.
    /// </summary>
    /// <param name="message">The message flowing through the pipeline.</param>
    /// <param name="handler">The terminal handler delegate.</param>
    /// <param name="cancellationToken">
    /// The token for the dispatch. A behavior that calls <c>next()</c> without a token gets this token,
    /// so cancellation keeps flowing through the chain.
    /// </param>
    /// <param name="innermostLayer">Behaviors closest to the handler (for example module-specific behaviors).</param>
    /// <param name="middleLayer">Behaviors wrapping the innermost layer, or <c>null</c>.</param>
    /// <param name="outermostLayer">Behaviors wrapping everything else, or <c>null</c>.</param>
    public static Task<TResponse> RunAsync<TMessage, TResponse>(
        TMessage message,
        PipelineDelegate<TResponse> handler,
        CancellationToken cancellationToken,
        IEnumerable<IPipelineBehavior<TMessage, TResponse>> innermostLayer,
        IEnumerable<IPipelineBehavior<TMessage, TResponse>>? middleLayer = null,
        IEnumerable<IPipelineBehavior<TMessage, TResponse>>? outermostLayer = null)
        where TMessage : IMessage
    {
        PipelineDelegate<TResponse> next = t => handler(t == default ? cancellationToken : t);

        next = Wrap(message, next, innermostLayer, cancellationToken);

        if (middleLayer is not null)
        {
            next = Wrap(message, next, middleLayer, cancellationToken);
        }

        if (outermostLayer is not null)
        {
            next = Wrap(message, next, outermostLayer, cancellationToken);
        }

        return next(cancellationToken);
    }

    /// <summary>
    /// Wraps <paramref name="next"/> with the behaviors of one layer, last registered innermost.
    /// </summary>
    private static PipelineDelegate<TResponse> Wrap<TMessage, TResponse>(
        TMessage message,
        PipelineDelegate<TResponse> next,
        IEnumerable<IPipelineBehavior<TMessage, TResponse>> layer,
        CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        // Microsoft.Extensions.DependencyInjection returns arrays for IEnumerable<T>; other containers may not.
        var behaviors = layer as IReadOnlyList<IPipelineBehavior<TMessage, TResponse>> ?? layer.ToArray();

        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var inner = next;
            next = t => behavior.HandleAsync(message, inner, t == default ? cancellationToken : t);
        }

        return next;
    }
}
