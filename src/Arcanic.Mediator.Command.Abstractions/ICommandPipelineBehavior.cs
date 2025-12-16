using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Defines a pipeline behavior interface for command handling, extending <see cref="IPipelineBehavior{TMessage,TMessageResult}"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled.</typeparam>
/// <typeparam name="TCommandResponse">The type of the response returned by the command handler.</typeparam>
public interface ICommandPipelineBehavior<in TCommand, TCommandResponse> : IPipelineBehavior<TCommand, TCommandResponse> where TCommand : ICommand { }