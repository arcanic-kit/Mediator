using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Mediator.Strategies;
using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query;

public class QueryMediator : IQueryMediator
{
    private readonly IMessageMediator _messageMediator;

    public QueryMediator(IMessageMediator messageMediator)
    {
        _messageMediator = messageMediator ?? throw new ArgumentNullException(nameof(messageMediator));
    }

    public async Task<TQueryResult> SendAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var strategy = new MessageMediatorSingleMainHandlerStrategy<IQuery<TQueryResult>, TQueryResult>();
        var options = new MessageMediatorOptions<IQuery<TQueryResult>, Task<TQueryResult>>()
        {
            Strategy = strategy,
            CancellationToken = cancellationToken,
        };

        return await _messageMediator.Mediate(query, options);
    }
}
