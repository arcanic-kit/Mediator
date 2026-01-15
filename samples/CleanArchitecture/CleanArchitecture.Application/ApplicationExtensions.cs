using Arcanic.Mediator.Command;
using Arcanic.Mediator.Event;
using Arcanic.Mediator.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Arcanic.Mediator;
using Arcanic.Mediator.Request;
using CleanArchitecture.Application.Common.PipelineBehaviors;

namespace CleanArchitecture.Application;

public static class ApplicationExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddArcanicMediator(config =>
        {
            config.Lifetime = ServiceLifetime.Scoped;
        })
        .AddPipelineBehavior(typeof(ExamplePipelineBehavior<,>))
        .AddRequestPipelineBehavior(typeof(ExampleRequestPipelineBehavior<,>))
        .AddCommandPipelineBehavior(typeof(ExampleCommandPipelineBehavior<,>))
        .AddQueryPipelineBehavior(typeof(ExampleQueryPipelineBehavior<,>))
        .AddEventPipelineBehavior(typeof(ExampleEventPipelineBehavior<,>))
        .AddCommands(Assembly.GetExecutingAssembly())
        .AddQueries(Assembly.GetExecutingAssembly())
        .AddEvents(Assembly.GetExecutingAssembly());

        return builder;
    }
}