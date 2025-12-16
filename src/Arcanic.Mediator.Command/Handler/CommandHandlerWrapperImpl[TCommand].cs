using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Command.Handler;

/// <summary>
///     Concrete implementation of <see cref="CommandHandlerWrapper"/> for handling commands of type <typeparamref name="TCommand"/>.
///     Resolves the appropriate <see cref="ICommandHandler{TCommand}"/> and applies any registered
///     <see cref="IPipelineBehavior{TMessage,TMessageResult}"/> instances in the pipeline.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
public class CommandHandlerWrapperImpl<TCommand> : CommandHandlerWrapper
    where TCommand : ICommand
{
    /// <summary>
    ///     Handles the specified command request using the provided service provider and cancellation token.
    ///     Casts the request to the appropriate command type and delegates to the strongly-typed handler.
    /// </summary>
    /// <param name="request">The command request to handle (as an object).</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((ICommand) request, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Handles the specified command request using the provided service provider and cancellation token.
    ///     Resolves the appropriate <see cref="ICommandHandler{TCommand}"/> and applies all registered
    ///     <see cref="IPipelineBehavior{TMessage,TMessageResult}"/> instances in reverse order, wrapping the handler.
    /// </summary>
    /// <param name="request">The command request to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with a result of type <see cref="Unit"/>.
    /// </returns>
    public override Task<Unit> Handle(ICommand request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Defines the core handler delegate that invokes the command handler.
        async Task<Unit> Handler(CancellationToken t = default)
        {
            await serviceProvider.GetRequiredService<ICommandHandler<TCommand>>()
                .HandleAsync((TCommand) request, t == default ? cancellationToken : t);

            return Unit.Value;
        }

        // Applies all pipeline behaviors in reverse order, wrapping the handler.
        return serviceProvider
            .GetServices<IPipelineBehavior<TCommand, Unit>>()
            .Reverse()
            .Aggregate((PipelineDelegate<Unit>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TCommand) request, next, t == default ? cancellationToken : t))();
    }
}
