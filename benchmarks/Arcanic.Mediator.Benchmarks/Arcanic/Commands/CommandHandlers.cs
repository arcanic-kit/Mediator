using Arcanic.Mediator.Benchmarks.Models;
using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Benchmarks.Arcanic.Commands;

/// <summary>
/// Arcanic command wrapper for CreateUserCommand
/// </summary>
public class CreateUserArcanicCommand : ICommand<CreateUserCommandResult>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Arcanic command wrapper for UpdateUserCommand
/// </summary>
public class UpdateUserArcanicCommand : ICommand
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Arcanic command wrapper for DeleteUserCommand
/// </summary>
public class DeleteUserArcanicCommand : ICommand
{
    public int Id { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Arcanic handler for CreateUserCommand
/// </summary>
public class CreateUserArcanicHandler : ICommandHandler<CreateUserArcanicCommand, CreateUserCommandResult>
{
    public Task<CreateUserCommandResult> HandleAsync(CreateUserArcanicCommand request, CancellationToken cancellationToken = default)
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
/// Arcanic handler for UpdateUserCommand
/// </summary>
public class UpdateUserArcanicHandler : ICommandHandler<UpdateUserArcanicCommand>
{
    public Task HandleAsync(UpdateUserArcanicCommand request, CancellationToken cancellationToken = default)
    {
        // Simulate user update
        return Task.CompletedTask;
    }
}

/// <summary>
/// Arcanic handler for DeleteUserCommand
/// </summary>
public class DeleteUserArcanicHandler : ICommandHandler<DeleteUserArcanicCommand>
{
    public Task HandleAsync(DeleteUserArcanicCommand request, CancellationToken cancellationToken = default)
    {
        // Simulate user deletion
        return Task.CompletedTask;
    }
}