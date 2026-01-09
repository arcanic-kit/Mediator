using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator;

public static class ServiceCollectionExtensions
{

    public static ArcanicMediatorBuilder AddArcanicMediator(this IServiceCollection services)
    {
        return new ArcanicMediatorBuilder(services);
    }
}
