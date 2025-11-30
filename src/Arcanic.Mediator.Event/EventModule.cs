using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Messaging.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Event;

/// <summary>
/// Provides a module for configuring event-based mediator services within the dependency injection container.
/// This module encapsulates the registration of event mediator components and their dependencies.
/// </summary>
public class EventModule: IModule
{
    private readonly Action<EventModuleBuilder> _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventModule"/> class with the specified configuration action.
    /// </summary>
    /// <param name="builder">An action that configures the event module builder to register event handlers and related services.
    /// This action receives an <see cref="EventModuleBuilder"/> instance for configuration.</param>
    public EventModule(Action<EventModuleBuilder> builder)
    {
        _builder = builder;
    }

    /// <summary>
    /// Configures the dependency injection container by registering event mediator services and executing the builder configuration.
    /// This method sets up the event infrastructure including the event mediator and processes any custom registrations
    /// specified through the builder action.
    /// </summary>
    /// <param name="services">The service collection to configure with event mediator registrations.</param>
    public void Build(IServiceCollection services)
    {
        var messageRegistry = (MessageRegistryAccessor.Instance);

        _builder(new EventModuleBuilder(services, messageRegistry));

        services.TryAddScoped<IEventPublisher, EventPublisher>();
    }
}
