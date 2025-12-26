using Arcanic.Mediator.Abstractions.Handler.Post;

namespace Arcanic.Mediator.Query.Abstractions.Handler;

/// <summary>
/// Defines a post-handler that executes after the main query handler.
/// Post-handlers are useful for implementing cross-cutting concerns such as logging,
/// caching, cleanup operations, or other follow-up activities.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
public interface IQueryPostHandler<TQuery> : IAsyncMessagePostHandler<TQuery> where TQuery : notnull;