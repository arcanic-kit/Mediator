using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.VoidCommand;

public class VoidCommand : ICommand
{
    public string Message { get; set; } = string.Empty;
}