using Arcanic.Mediator.Benchmarks.Models;
using MediatR;

namespace Arcanic.Mediator.Benchmarks.MediatR.Commands;

/// <summary>
/// MediatR wrapper for CreateUserCommand
/// </summary>
public record CreateUserRequest(string Name, string Email) : IRequest<CreateUserCommandResult>;

/// <summary>
/// MediatR wrapper for UpdateUserCommand
/// </summary>
public record UpdateUserRequest(int Id, string Name, string Email) : IRequest;

/// <summary>
/// MediatR wrapper for DeleteUserCommand
/// </summary>
public record DeleteUserRequest(int Id, string Reason) : IRequest;

/// <summary>
/// MediatR handler for CreateUserCommand
/// </summary>
public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserCommandResult>
{
    public Task<CreateUserCommandResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        // Simulate user creation
        var result = new CreateUserCommandResult
        {
            Id = Random.Shared.Next(1, 1000),
            Success = true
        };

        return Task.FromResult(result);
    }
}

/// <summary>
/// MediatR handler for UpdateUserCommand
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserRequest>
{
    public Task Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        // Simulate user update
        return Task.CompletedTask;
    }
}

/// <summary>
/// MediatR handler for DeleteUserCommand
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserRequest>
{
    public Task Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        // Simulate user deletion
        return Task.CompletedTask;
    }
}