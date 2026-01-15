using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Request.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Command.Dispatcher;

/// <summary>
/// Abstract base class for command dispatchers.
/// Provides an interface for dispatching commands with dynamic command types and dependency injection.
/// </summary>
public abstract class CommandDispatcherBase
{
    /// <summary>
    /// Dispatches the specified command using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with an optional result object.
    /// </returns>
    public abstract Task<object?> DispatchAsync(object command, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves and combines all pipeline behaviors that can be applied to the specified command and response types.
    /// This method aggregates command-specific pipeline behaviors, request pipeline behaviors, and generic pipeline behaviors
    /// in order of precedence, with behaviors reversed to ensure proper execution order.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being processed. Must implement <see cref="ICommand"/>.</typeparam>
    /// <typeparam name="TResponse">The type of response returned by the command handler.</typeparam>
    /// <param name="serviceProvider">The service provider used to resolve pipeline behavior instances from the dependency injection container.</param>
    /// <returns>
    /// An enumerable collection of pipeline behaviors that will be applied to the command processing pipeline.
    /// The behaviors are returned in execution order: command-specific behaviors first, followed by request behaviors, then generic pipeline behaviors.
    /// </returns>
    protected IEnumerable<IPipelineBehavior<TCommand, TResponse>> GetAllPipelineBehaviors<TCommand, TResponse>(IServiceProvider serviceProvider)
        where TCommand : ICommand
    {
        var commandPipelineBehaviors = serviceProvider
            .GetServices<ICommandPipelineBehavior<TCommand, TResponse>>()
            .Reverse();
        
        var requestedPipelineBehaviors = serviceProvider
            .GetServices<IRequestPipelineBehavior<TCommand, TResponse>>()
            .Reverse();
        
        var pipelineBehaviors = serviceProvider
            .GetServices<IPipelineBehavior<TCommand, TResponse>>()
            .Reverse();

        return commandPipelineBehaviors
            .Cast<IPipelineBehavior<TCommand, TResponse>>()
            .Concat(requestedPipelineBehaviors)
            .Concat(pipelineBehaviors);
    }
}
