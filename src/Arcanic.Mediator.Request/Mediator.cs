using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Dispatcher;
using System.Collections.Concurrent;

namespace Arcanic.Mediator.Request;

public class Mediator : IMediator
{
    /// <summary>
    /// The service provider used for dependency injection and handler resolution.
    /// </summary>
    public IServiceProvider ServiceProvider { get; set; }

    public ConcurrentDictionary<Type, RequestDispatcherBase> RequestDispatchers { get; set; } = new();

    public Mediator(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
}
