namespace Arcanic.Mediator.Query.Abstractions;

/// <summary>
/// Defines a contract for executing queries within the mediator framework.
/// The query mediator provides a centralized mechanism for processing queries
/// and returning their results using a request-response pattern.
/// </summary>
public interface IQueryMediator
{
    /// <summary>
    /// Asynchronously executes a query and returns the result.
    /// </summary>
    /// <typeparam name="TQueryResult">The type of result returned by the query.</typeparam>
    /// <param name="command">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous query execution with the result.</returns>
    Task<TQueryResult> SendAsync<TQueryResult>(IQuery<TQueryResult> command, CancellationToken cancellationToken = default);
}
