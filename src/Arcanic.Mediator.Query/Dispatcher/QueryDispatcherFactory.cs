using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Request.Dispatcher;

namespace Arcanic.Mediator.Query.Dispatcher;

/// <summary>
/// Creates <see cref="QueryDispatcher{TQuery, TResponse}"/> instances for query types.
/// </summary>
internal sealed class QueryDispatcherFactory : IRequestDispatcherFactory
{
    /// <inheritdoc />
    public RequestDispatcherBase? CreateDispatcher(Type requestType)
    {
        var resultInterfaces = requestType.GetInterfaces()
            .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQuery<>))
            .ToArray();

        return resultInterfaces.Length switch
        {
            0 => null,
            1 => (RequestDispatcherBase)Activator.CreateInstance(
                typeof(QueryDispatcher<,>).MakeGenericType(requestType, resultInterfaces[0].GetGenericArguments()[0]))!,
            _ => throw new InvalidOperationException(
                $"Query type '{requestType.FullName}' implements IQuery<T> more than once; a query must have a single result type.")
        };
    }
}
