using Arcanic.Mediator.Request.Abstractions.Dispatcher;
using System.Collections.Concurrent;

namespace Arcanic.Mediator.Request.Abstractions;

public interface IMediator
{
    IServiceProvider ServiceProvider { get; }

    ConcurrentDictionary<Type, RequestDispatcherBase> RequestDispatchers { get; }
}
