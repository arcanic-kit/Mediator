using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.UnhandledCommand;

public class UnhandledCommand : ICommand<UnhandledCommandResponse>
{
    public string Message { get; set; } = string.Empty;
}