using Arcanic.Mediator.Abstractions.Handler.Pre;

namespace Arcanic.Mediator.Query.Abstractions;

/// <summary>
/// Defines a pre-handler that executes before the main query handler.
/// Pre-handlers are useful for implementing cross-cutting concerns such as validation,
/// authentication, logging, or other preparatory operations.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
public interface IQueryPreHandler<TQuery> : IAsyncMessagePreHandler<TQuery> where TQuery : notnull;