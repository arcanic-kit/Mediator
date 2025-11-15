using Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Defines a handler for processing commands that do not return a result.
/// Command handlers are responsible for executing the business logic associated with a command
/// and performing the necessary operations to complete the command's intent.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandHandler<TCommand> : IAsyncMessageMainHandler<TCommand> where TCommand : ICommand;
