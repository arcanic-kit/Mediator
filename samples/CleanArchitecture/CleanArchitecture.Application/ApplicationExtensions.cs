using Arcanic.Mediator.Command;
using Arcanic.Mediator.Event;
using Arcanic.Mediator.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Arcanic.Mediator;
using Arcanic.Mediator.Abstractions.Pipeline;
using CleanArchitecture.Application.Common.PipelineBehaviors;

namespace CleanArchitecture.Application;

public static class ApplicationExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // Example: Add Arcanic Mediator with modules
        builder.Services.AddArcanicMediator(moduleRegistry =>
        {
            moduleRegistry.AddCommandModule(commandBuilder =>
            {
                commandBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });

            moduleRegistry.AddQueryModule(queryBuilder =>
            {
                queryBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });

            moduleRegistry.AddEventModule(eventBuilder =>
            {
                eventBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });
        });

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceMonitoringPipelineBehavior<,>));

        return builder;
    }
}