using Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

namespace Arcanic.Mediator.Query.Abstractions;

public interface IQueryHandler<TQuery, TQueryResult> : IAsyncMessageHandler<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>;

