using Arcanic.Mediator.Sample.WebApi.Application;
using Arcanic.Mediator.Sample.WebApi.Application.Common.Repositories;
using Arcanic.Mediator.Sample.WebApi.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arcanic.Mediator.Sample.WebApi.Infrastructure;

public static class InfrastructureExtensions
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddApplicationServices();

        builder.Services.AddScoped<IProductRepository, ProductRepository>();

        return builder;
    }
}