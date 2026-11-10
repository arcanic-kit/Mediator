namespace Arcanic.Mediator.Request.Dispatcher;

/// <summary>
/// Creates the dispatcher for a request type. Each request module (commands, queries) registers one factory;
/// <see cref="Mediator"/> asks them in turn the first time it sees a request type and caches the result.
/// </summary>
public interface IRequestDispatcherFactory
{
    /// <summary>
    /// Creates a dispatcher for <paramref name="requestType"/>.
    /// </summary>
    /// <param name="requestType">The concrete request type being dispatched.</param>
    /// <returns>
    /// A dispatcher for the request type, or <c>null</c> when this factory does not handle that kind of request.
    /// </returns>
    RequestDispatcherBase? CreateDispatcher(Type requestType);
}
