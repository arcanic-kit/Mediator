using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Handler;

namespace Arcanic.Mediator.Query.Abstractions;

/// <summary>
/// Represents a marker interface for queries within the mediator framework.
/// Queries are read-only operations that retrieve data and return a specific result type.
/// </summary>
/// <typeparam name="TQueryResult">The type of result returned by the query.</typeparam>
/// <remarks>
/// This interface serves as a contract for all query types in the application.
/// Queries implementing this interface can be executed through the <see cref="IQueryMediator"/>
/// and will be processed by a single handler that implements <see cref="IQueryHandler{TQuery,TResult}"/>
/// for the specific query and result types.
/// </remarks>
public interface IQuery<TQueryResult> : IRequest;

