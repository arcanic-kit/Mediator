using Arcanic.Mediator.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Abstractions.Pipeline;

namespace Arcanic.Mediator.Command.Pipeline;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="ICommandPostHandler{T}"/> for the post-handler stage of a command.
/// </summary>
/// <typeparam name="TRequest">The command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class CommandPostHandlerPipelineBehavior<TRequest, TResponse>
    : PostHandlerPipelineBehaviorBase<TRequest, TResponse, ICommandPostHandler<TRequest>>, ICommandPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandPostHandlerPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="postHandlers">The post-handlers registered for <typeparamref name="TRequest"/>.</param>
    public CommandPostHandlerPipelineBehavior(IEnumerable<ICommandPostHandler<TRequest>> postHandlers)
        : base(postHandlers)
    {
    }
}
