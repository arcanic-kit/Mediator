using CleanArchitecture.Application;
using CleanArchitecture.Domain.Products;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArchitecture.Infrastructure;

public static class InfrastructureExtensions
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddApplicationServices();

        builder.Services.AddScoped<IProductRepository, ProductRepository>();

        return builder;
    }
}