using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.Error;

public class ErrorCommand : ICommand<ErrorCommandResponse>
{
    public string ErrorMessage { get; set; } = string.Empty;
}