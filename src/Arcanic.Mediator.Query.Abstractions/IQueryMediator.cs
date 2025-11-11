namespace Arcanic.Mediator.Query.Abstractions;

public interface IQueryMediator
{
    Task<TQueryResult> SendAsync<TQueryResult>(IQuery<TQueryResult> command, CancellationToken cancellationToken = default);
}
