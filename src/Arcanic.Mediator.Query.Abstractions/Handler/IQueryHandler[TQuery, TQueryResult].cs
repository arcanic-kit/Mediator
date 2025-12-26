using Arcanic.Mediator.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Query.Abstractions.Handler;

/// <summary>
/// Defines a contract for handling queries of a specific type within the mediator framework.
/// Query handlers implement the request-response pattern, where a single handler processes
/// a query and returns a result.
/// </summary>
/// <typeparam name="TQuery">The type of query this handler can process. The query type must implement <see cref="IQuery{TQueryResult}"/>.</typeparam>
/// <typeparam name="TQueryResult">The type of result returned by the query handler.</typeparam>
/// <remarks>
/// Query handlers are automatically discovered and registered when using assembly scanning
/// functionality. Each handler is registered as a scoped service in the dependency injection
/// container and will be invoked when the corresponding query type is executed through the <see cref="IQueryMediator"/>.
/// </remarks>
public interface IQueryHandler<TQuery, TQueryResult> : IAsyncMessageHandler<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>;

