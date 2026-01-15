using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Command.Dispatcher;

/// <summary>
/// Concrete implementation of command dispatcher for a specific command type.
/// Handles the dispatch of the appropriate command handler and applies any registered pipeline behaviors.
/// </summary>
/// <typeparam name="TCommand">The type of command to dispatch.</typeparam>
public class CommandDispatcher<TCommand> : CommandDispatcherBase
    where TCommand : ICommand
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
        await DispatchAsync((ICommand) command, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Dispatches the specified command using the provided service provider and cancellation token.
    /// Resolves the appropriate command handler and applies all registered pipeline behaviors in reverse order.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of type <see cref="Unit"/>.
    /// </returns>
    private Task<Unit> DispatchAsync(ICommand command, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Defines the core handler delegate that invokes the command handler.
        async Task<Unit> Handler(CancellationToken t = default)
        {
            await serviceProvider.GetRequiredService<ICommandHandler<TCommand>>()
                .HandleAsync((TCommand) command, t == default ? cancellationToken : t);

            return Unit.Value;
        }
        
        return GetAllPipelineBehaviors<TCommand, Unit>(serviceProvider)
            .Aggregate((PipelineDelegate<Unit>) Handler,
                (next, pipeline) => (t) => pipeline.HandleAsync((TCommand) command, next, t == default ? cancellationToken : t))();
    }
}
