using System;
using System.Threading;
using System.Threading.Tasks;
using Arcanic.Mediator.Command.Abstractions.Handler;

namespace Arcanic.Mediator.Command.Benchmarks.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserCommandResponse>
{
    public Task<CreateUserCommandResponse> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        // Simulate user creation
        var result = new CreateUserCommandResponse
        {
            Id = Random.Shared.Next(1, 1000),
            Success = true
        };

        return Task.FromResult(result);
    }
}