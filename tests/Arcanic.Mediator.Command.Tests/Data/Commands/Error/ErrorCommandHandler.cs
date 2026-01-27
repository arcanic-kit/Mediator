using Arcanic.Mediator.Command.Abstractions.Handler;

namespace Arcanic.Mediator.Command.Tests.Data.Commands.Error;

public class ErrorCommandHandler : ICommandHandler<ErrorCommand, ErrorCommandResponse>
{
    public Task<ErrorCommandResponse> HandleAsync(ErrorCommand command, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(command.ErrorMessage);
    }
}