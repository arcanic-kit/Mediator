using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator;

public class ArcanicMediatorBuilder: IArcanicMediatorBuilder
{
    public IServiceCollection Services { get; }
    
    public ArcanicMediatorBuilder(IServiceCollection services)
    {
        Services = services;
    }
    
    public IArcanicMediatorBuilder Configure()
    {
        return this;
    }
    
    public IArcanicMediatorBuilder AddPipelineBehavior(Type implementationType)
    {
        Services.AddTransient(typeof(IPipelineBehavior<,>), implementationType);
        
        return this;
    }
}