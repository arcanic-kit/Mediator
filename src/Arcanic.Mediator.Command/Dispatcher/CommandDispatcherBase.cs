using Arcanic.Mediator.Request.Dispatcher;

namespace Arcanic.Mediator.Command.Dispatcher;

/// <summary>
/// Abstract base class for command dispatchers.
/// Provides an interface for dispatching commands with dynamic command types and dependency injection.
/// </summary>
public abstract class CommandDispatcherBase: RequestDispatcherBase
{
}
