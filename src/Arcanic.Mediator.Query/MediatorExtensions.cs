using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Dispatcher;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Dispatcher;

namespace Arcanic.Mediator.Query;

public static class MediatorExtensions
{
    public static async Task<TQueryResponse> SendAsync<TQueryResponse>(this IMediator mediator, IQuery<TQueryResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryType = query.GetType();

        // Retrieve or create dispatcher using thread-safe GetOrAdd
        var dispatcher = mediator.RequestDispatchers.GetOrAdd(queryType, static requestType =>
        {
            // Create typed dispatcher: QueryDispatcher<TQuery, TQueryResponse>
            var dispatcherType = typeof(QueryDispatcher<,>).MakeGenericType(requestType, typeof(TQueryResponse));
            var dispatcher = Activator.CreateInstance(dispatcherType) ?? throw new InvalidOperationException($"Could not create dispatcher type for {requestType}");
            return (RequestDispatcherBase)dispatcher;
        });

        // Execute query through dispatcher and return typed result
        var result = await dispatcher.DispatchAsync(query, mediator.ServiceProvider, cancellationToken);
        return (TQueryResponse)result!;
    }
}
