using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Command.Dispatcher;

/// <summary>
/// Concrete implementation of command dispatcher for a specific command and response type.
/// Handles the dispatch of the appropriate command handler and applies any registered pipeline behaviors.
/// </summary>
/// <typeparam name="TCommand">The type of command to dispatch.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the command.</typeparam>
public class CommandDispatcher<TCommand, TResponse> : CommandDispatcherBase
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Dispatches the specified command using the provided service provider and cancellation token.
    /// Casts the command to the appropriate type and delegates to the strongly-typed dispatcher.
    /// </summary>
    /// <param name="command">The command to dispatch (as an object).</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public override async Task<object?> DispatchAsync(object command, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await DispatchAsync((ICommand<TResponse>) command, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Dispatches the specified command using the provided service provider and cancellation token.
    /// Resolves the command handler and applies all registered pipeline behaviors in reverse order.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <typeparamref name="TResponse"/>.
    /// </returns>
    private Task<TResponse> DispatchAsync(ICommand<TResponse> command, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Defines the core handler delegate that invokes the command handler.
        Task<TResponse> Handler(CancellationToken t = default) => serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>()
            .HandleAsync((TCommand) command, t == default ? cancellationToken : t);

        var allPipelineBehaviors = serviceProvider
            .GetServices<ICommandPipelineBehavior<TCommand, TResponse>>()
            .Cast<IPipelineBehavior<TCommand, TResponse>>()
            .Concat(serviceProvider.GetServices<IRequestPipelineBehavior<TCommand, TResponse>>())
            .Concat(serviceProvider.GetServices<IPipelineBehavior<TCommand, TResponse>>());

        return allPipelineBehaviors
            .Aggregate((PipelineDelegate<TResponse>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TCommand) command, next, t == default ? cancellationToken : t))();
    }
}
