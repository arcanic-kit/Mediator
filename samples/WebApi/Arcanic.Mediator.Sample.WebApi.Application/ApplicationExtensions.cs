using Arcanic.Mediator.Command;
using Arcanic.Mediator.Query;
using Arcanic.Mediator.Event;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Arcanic.Mediator.Sample.WebApi.Application;

public static class ApplicationExtensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // Example: Add Arcanic Mediator with modules
        builder.Services.AddArcanicMediator(moduleRegistry =>
        {
            moduleRegistry.AddCommandModule(commandModuleBuilder =>
            {
                commandModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });

            moduleRegistry.AddQueryModule(queryModuleBuilder =>
            {
                queryModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });

            moduleRegistry.AddEventModule(eventModuleBuilder =>
            {
                eventModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
            });
        });

        return builder;
    }
}