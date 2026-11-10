using Arcanic.Mediator.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Handler;
using Arcanic.Mediator.Command.Abstractions.Pipeline;

namespace Arcanic.Mediator.Command.Pipeline;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="ICommandPreHandler{T}"/> for the pre-handler stage of a command.
/// </summary>
/// <typeparam name="TRequest">The command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class CommandPreHandlerPipelineBehavior<TRequest, TResponse>
    : PreHandlerPipelineBehaviorBase<TRequest, TResponse, ICommandPreHandler<TRequest>>, ICommandPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandPreHandlerPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="preHandlers">The pre-handlers registered for <typeparamref name="TRequest"/>.</param>
    public CommandPreHandlerPipelineBehavior(IEnumerable<ICommandPreHandler<TRequest>> preHandlers)
        : base(preHandlers)
    {
    }
}
