namespace Arcanic.Mediator.Benchmarks.Models;

/// <summary>
/// Simple command for benchmarking
/// </summary>
public class CreateUserCommand
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Command result
/// </summary>
public class CreateUserCommandResult
{
    public int Id { get; init; }
    public bool Success { get; init; }
}

/// <summary>
/// Command without result
/// </summary>
public class UpdateUserCommand
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Complex command with validation
/// </summary>
public class DeleteUserCommand
{
    public int Id { get; init; }
    public string Reason { get; init; } = string.Empty;
}