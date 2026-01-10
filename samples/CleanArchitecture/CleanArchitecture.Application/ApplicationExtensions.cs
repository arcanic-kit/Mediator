using Arcanic.Mediator.Command;
using Arcanic.Mediator.Event;
using Arcanic.Mediator.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Arcanic.Mediator;
using Arcanic.Mediator.Abstractions.Pipeline;
using CleanArchitecture.Application.Common.PipelineBehaviors;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

public static class ApplicationExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddArcanicMediator(config =>
        {
                config.Lifetime = ServiceLifetime.Scoped;
            })
            .AddPipelineBehavior(typeof(LoggingPipelineBehavior<,>))
            .AddPipelineBehavior(typeof(PerformanceMonitoringPipelineBehavior<,>))
            .AddCommands(Assembly.GetExecutingAssembly())
            .AddQueries(Assembly.GetExecutingAssembly())
            .AddEvents(Assembly.GetExecutingAssembly());

        return builder;
    }
}