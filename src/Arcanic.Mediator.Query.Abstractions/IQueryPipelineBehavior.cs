using Arcanic.Mediator.Abstractions.Pipeline;

namespace Arcanic.Mediator.Query.Abstractions;

/// <summary>
/// Defines a pipeline behavior interface for query handling, extending <see cref="IPipelineBehavior{TQuery, TQueryResponse}"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the query being handled.</typeparam>
/// <typeparam name="TQueryResponse">The type of the response returned by the query handler.</typeparam>
public interface IQueryPipelineBehavior<in TQuery, TQueryResponse> : IPipelineBehavior<TQuery, TQueryResponse> where TQuery : IQuery<TQueryResponse> { }