using Arcanic.Mediator.Abstractions.Modules;
using Arcanic.Mediator.Messaging.Abstractions.Mediator;
using Arcanic.Mediator.Messaging.Mediator;
using Arcanic.Mediator.Messaging.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcanic.Mediator.Messaging;

/// <summary>
/// Provides a module for configuring message-based mediation services within the dependency injection container.
/// This module registers the message registry, mediator services, and allows configuration of message-handler mappings
/// through a fluent builder pattern.
/// </summary>
public class MessageModule : IModule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageModule"/> class.
    /// </summary>
    public MessageModule() {}

    /// <summary>
    /// Configures the dependency injection container with message mediation services.
    /// This method registers the message registry, creates and configures the module builder,
    /// and registers the core mediator services required for message processing.
    /// </summary>
    /// <param name="services">The service collection to configure with message mediation services.</param>
    public void Build(IServiceCollection services)
    {
        var messageRegistry = (MessageRegistryAccessor.Instance);

        services.TryAddSingleton(messageRegistry);
        services.TryAddTransient<IMessageMediator, MessageMediator>();
    }
}
