using Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Command.Abstractions;

public interface ICommandHandler<TCommand> : IAsyncMessageMainHandler<TCommand> where TCommand : ICommand;

public interface ICommandHandler<TCommand, TCommandResult> : IAsyncMessageHandler<TCommand, TCommandResult> where TCommand : ICommand<TCommandResult>;
