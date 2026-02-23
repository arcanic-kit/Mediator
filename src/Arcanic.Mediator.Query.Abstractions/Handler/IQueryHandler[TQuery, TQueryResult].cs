using Arcanic.Mediator.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Query.Abstractions.Handler;

/// <summary>
/// Defines a contract for handling queries of a specific type within the mediator framework.
/// Query handlers implement the request-response pattern, where a single handler processes
/// a query and returns a result.
/// </summary>
/// <typeparam name="TQuery">The type of query this handler can process. The query type must implement <see cref="IQuery{TQueryResult}"/>.</typeparam>
/// <typeparam name="TQueryResult">The type of result returned by the query handler.</typeparam>
public interface IQueryHandler<TQuery, TQueryResult> : IAsyncMessageHandler<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>;

