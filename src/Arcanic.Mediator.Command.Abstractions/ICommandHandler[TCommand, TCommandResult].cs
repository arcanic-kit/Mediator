using Arcanic.Mediator.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Defines a handler for processing commands that return a result of the specified type.
/// Command handlers are responsible for executing the business logic associated with a command
/// and returning an appropriate result upon completion.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand{TCommandResult}"/>.</typeparam>
/// <typeparam name="TCommandResult">The type of result returned after processing the command.</typeparam>
public interface ICommandHandler<TCommand, TCommandResult> : IAsyncMessageHandler<TCommand, TCommandResult> where TCommand : ICommand<TCommandResult>;
