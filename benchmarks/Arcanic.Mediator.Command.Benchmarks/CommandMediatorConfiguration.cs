using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Command.Benchmarks;

/// <summary>
/// Configuration class for Arcanic Command Mediator services
/// </summary>
public static class CommandMediatorConfiguration
{
    /// <summary>
    /// Registers Arcanic Command Mediator services with dependency injection
    /// </summary>
    public static IServiceCollection AddArcanicCommandMediator(this IServiceCollection services)
    {
        services.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());

        return services;
    }
}
