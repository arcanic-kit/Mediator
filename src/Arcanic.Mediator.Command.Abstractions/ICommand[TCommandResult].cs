namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Represents a command that can be processed by the command mediator and returns a result.
/// Commands are messages that represent an intent to perform an action or operation
/// that may modify system state and produce a return value of the specified type.
/// </summary>
/// <typeparam name="TCommandResult">The type of result returned by the command processing.</typeparam>
public interface ICommand<TCommandResult> : ICommand;
