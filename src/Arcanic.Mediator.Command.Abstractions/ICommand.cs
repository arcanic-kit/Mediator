using Arcanic.Mediator.Abstractions;

namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Represents a command that can be processed by the command mediator.
/// Commands are messages that represent an intent to perform an action or operation
/// that may modify system state. They are processed by command handlers and do not return a result.
/// </summary>
public interface ICommand : IRequest;
