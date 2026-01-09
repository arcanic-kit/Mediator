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
        builder.Services.AddArcanicMediator()
            .Configure()
            .AddPipelineBehavior(typeof(LoggingPipelineBehavior<,>))
            .AddPipelineBehavior(typeof(PerformanceMonitoringPipelineBehavior<,>))
            .AddCommands(Assembly.GetExecutingAssembly())
            .AddQueries(Assembly.GetExecutingAssembly())
            .AddEvents(Assembly.GetExecutingAssembly());
        
        // builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        // builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceMonitoringPipelineBehavior<,>));

        return builder;
    }
}