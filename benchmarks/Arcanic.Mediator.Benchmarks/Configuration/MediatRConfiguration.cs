using Arcanic.Mediator.Benchmarks.MediatR.Commands;
using Arcanic.Mediator.Benchmarks.MediatR.Events;
using Arcanic.Mediator.Benchmarks.MediatR.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Benchmarks.Configuration;

/// <summary>
/// Configuration class for MediatR services
/// </summary>
public static class MediatRConfiguration
{
    /// <summary>
    /// Registers MediatR services with dependency injection
    /// </summary>
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<GetUserHandler>();
            cfg.RegisterServicesFromAssemblyContaining<CreateUserHandler>();
            cfg.RegisterServicesFromAssemblyContaining<UserCreatedEmailHandler>();
        });

        return services;
    }
}