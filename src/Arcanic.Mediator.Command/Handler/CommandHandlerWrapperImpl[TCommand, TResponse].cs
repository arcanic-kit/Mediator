using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Command.Handler;

/// <summary>
///     A concrete implementation of <see cref="CommandHandlerWrapper{TResponse}"/> for handling commands of type <typeparamref name="TCommand"/>
///     and producing responses of type <typeparamref name="TResponse"/>.
///     This class resolves the appropriate <see cref="ICommandHandler{TCommand, TResponse}"/> and applies any registered
///     <see cref="IPipelineBehavior{TMessage,TMessageResult}"/> instances in the pipeline.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler.</typeparam>
public class CommandHandlerWrapperImpl<TCommand, TResponse> : CommandHandlerWrapper<TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    ///     Handles the specified command request using the provided service provider and cancellation token.
    ///     Casts the request to the appropriate command type and delegates to the strongly-typed handler.
    /// </summary>
    /// <param name="command">The command to handle (as an object).</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public override async Task<object?> Handle(object command, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((ICommand<TResponse>) command, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    ///     Handles the specified command request using the provided service provider and cancellation token.
    ///     Resolves the appropriate <see cref="ICommandHandler{TCommand, TResponse}"/> and applies all registered
    ///     <see cref="IPipelineBehavior{TMessage,TMessageResult}"/> instances in reverse order, wrapping the handler.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with a result of type <typeparamref name="TResponse"/>.
    /// </returns>
    public override Task<TResponse> Handle(ICommand command, IServiceProvider serviceProvider, CancellationToken cancellationToken)
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