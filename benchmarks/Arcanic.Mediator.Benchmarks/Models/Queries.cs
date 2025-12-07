namespace Arcanic.Mediator.Benchmarks.Models;

/// <summary>
/// Simple query model for benchmarking purposes
/// </summary>
public class GetUserQuery
{
    public int Id { get; init; }
}

/// <summary>
/// Query result model
/// </summary>
public class GetUserQueryResult
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Complex query with multiple parameters
/// </summary>
public class SearchUsersQuery
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// Search result with multiple items
/// </summary>
public class SearchUsersQueryResult
{
    public IEnumerable<GetUserQueryResult> Users { get; init; } = [];
    public int TotalCount { get; init; }
}