using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Abstractions;

public interface IArcanicMediatorBuilder
{
    IServiceCollection Services { get; }
    IArcanicMediatorBuilder Configure();
    IArcanicMediatorBuilder AddPipelineBehavior(Type implementationType);
}