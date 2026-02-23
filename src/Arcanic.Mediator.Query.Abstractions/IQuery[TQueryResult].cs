using Arcanic.Mediator.Request.Abstractions;

namespace Arcanic.Mediator.Query.Abstractions;

/// <summary>
/// Represents a marker interface for queries within the mediator framework.
/// Queries are read-only operations that retrieve data and return a specific result type.
/// </summary>
/// <typeparam name="TQueryResult">The type of result returned by the query.</typeparam>
public interface IQuery<TQueryResult> : IRequest;

