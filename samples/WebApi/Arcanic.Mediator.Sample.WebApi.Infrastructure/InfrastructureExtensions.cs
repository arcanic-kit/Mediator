using Arcanic.Mediator.Sample.WebApi.Application;
using Microsoft.Extensions.Hosting;

namespace Arcanic.Mediator.Sample.WebApi.Infrastructure;

public static class InfrastructureExtensions
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddApplicationServices();

        return builder;
    }
}