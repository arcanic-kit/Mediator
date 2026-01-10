using System.Reflection;
using Arcanic.Mediator.Abstractions;

namespace Arcanic.Mediator.Event;

/// <summary>
/// Provides extension methods for <see cref="IArcanicMediatorBuilder"/> to register event handlers and related services.
/// </summary>
public static class ArcanicMediatorBuilderExtensions
{
    /// <summary>
    /// Registers all event handlers found in the specified assembly along with required event services.
    /// This method scans the assembly for event implementations and registers them with the dependency injection container.
    /// </summary>
    /// <param name="builder">The mediator builder instance to extend.</param>
    /// <param name="assembly">The assembly to scan for event handlers.</param>
    /// <returns>The mediator builder instance to enable method chaining.</returns>
    public static IArcanicMediatorBuilder AddEvents(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        var eventServiceRegistrar = new EventServiceRegistrar(builder.Services, builder.Configuration);

        eventServiceRegistrar.RegisterEventsFromAssembly(assembly);
        eventServiceRegistrar.RegisterRequiredServices();

        return builder;
    }
}