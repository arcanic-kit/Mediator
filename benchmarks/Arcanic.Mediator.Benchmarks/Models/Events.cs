namespace Arcanic.Mediator.Benchmarks.Models;

/// <summary>
/// Simple event for benchmarking
/// </summary>
public class UserCreatedEvent
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Event with more complex data
/// </summary>
public class UserUpdatedEvent
{
    public int Id { get; init; }
    public string OldName { get; init; } = string.Empty;
    public string NewName { get; init; } = string.Empty;
    public string OldEmail { get; init; } = string.Empty;
    public string NewEmail { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Event for deletion
/// </summary>
public class UserDeletedEvent
{
    public int Id { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
}