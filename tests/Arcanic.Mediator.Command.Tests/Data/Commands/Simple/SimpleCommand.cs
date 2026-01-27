using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.Simple;

public class SimpleCommand : ICommand<SimpleCommandResponse>
{
    public int Value { get; set; }
}