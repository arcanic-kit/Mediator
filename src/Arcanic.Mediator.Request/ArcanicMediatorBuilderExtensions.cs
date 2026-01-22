using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;
using Arcanic.Mediator.Request.DependencyInjection;

namespace Arcanic.Mediator.Request;

/// <summary>
/// Provides extension methods for configuring request pipeline behaviors in the Arcanic Mediator.
/// </summary>
public static class ArcanicMediatorBuilderExtensions
{
    /// <summary>
    /// Adds a request pipeline behavior to the mediator builder's service collection.
    /// </summary>
    /// <param name="builder">The Arcanic mediator builder instance.</param>
    /// <param name="requestPipelineBehaviorType">The type of the request pipeline behavior to add. Must implement <see cref="IRequestPipelineBehavior{TRequest, TResponse}"/>.</param>
    /// <returns>The same <see cref="IArcanicMediatorBuilder"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="requestPipelineBehaviorType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the specified type does not implement the required interface, is abstract, or is an interface.</exception>
    public static IArcanicMediatorBuilder AddRequestPipelineBehavior(this IArcanicMediatorBuilder builder,
        Type requestPipelineBehaviorType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(requestPipelineBehaviorType);

        builder.ServiceRegistrar.RegisterRequestPipelineBehavior(requestPipelineBehaviorType);

        return builder;
    }
}
