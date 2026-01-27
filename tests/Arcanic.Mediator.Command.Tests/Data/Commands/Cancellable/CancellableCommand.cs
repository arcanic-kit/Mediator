using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.Cancellable;

public class CancellableCommand : ICommand<CancellableCommandResponse>
{
    public int DelayMilliseconds { get; set; }
}