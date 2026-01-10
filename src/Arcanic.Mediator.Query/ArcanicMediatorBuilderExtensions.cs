using System.Reflection;
using Arcanic.Mediator.Abstractions;

namespace Arcanic.Mediator.Query;

/// <summary>
/// Provides extension methods for <see cref="IArcanicMediatorBuilder"/> to register query handlers and related services.
/// </summary>
public static class ArcanicMediatorBuilderExtensions
{
    /// <summary>
    /// Registers all query handlers found in the specified assembly along with required query services.
    /// This method scans the assembly for query implementations and registers them with the dependency injection container.
    /// </summary>
    /// <param name="builder">The mediator builder instance to extend.</param>
    /// <param name="assembly">The assembly to scan for query handlers.</param>
    /// <returns>The mediator builder instance to enable method chaining.</returns>
    public static IArcanicMediatorBuilder AddQueries(this IArcanicMediatorBuilder builder, Assembly assembly)
    {
        var queryServiceRegistrar = new QueryServiceRegistrar(builder.Services, builder.Configuration);

        queryServiceRegistrar.RegisterQueriesFromAssembly(assembly);
        queryServiceRegistrar.RegisterRequiredServices();

        return builder;
    }
}