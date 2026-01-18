using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;
using Microsoft.Extensions.DependencyInjection;

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

        ValidateRequestPipelineBehaviorType(requestPipelineBehaviorType);
        
        builder.Services.Add(new ServiceDescriptor(typeof(IRequestPipelineBehavior<,>), requestPipelineBehaviorType, builder.Configuration.InstanceLifetime));

        return builder;
    }

    /// <summary>
    /// Validates that the specified type implements the <see cref="IRequestPipelineBehavior{TRequest, TResponse}"/> interface
    /// and meets the requirements for registration as a pipeline behavior.
    /// </summary>
    /// <param name="requestPipelineBehaviorType">The type to validate.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the type does not implement <see cref="IRequestPipelineBehavior{TRequest, TResponse}"/> interface,
    /// is abstract, or is an interface type.
    /// </exception>
    private static void ValidateRequestPipelineBehaviorType(Type requestPipelineBehaviorType)
    {
        // Check if the type implements any generic version of IRequestPipelineBehavior<,>
        var implementsIRequestPipelineBehavior = requestPipelineBehaviorType
            .GetInterfaces()
            .Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IRequestPipelineBehavior<,>));

        if (!implementsIRequestPipelineBehavior)
        {
            throw new ArgumentException(
                $"Type '{requestPipelineBehaviorType.FullName}' must implement '{typeof(IRequestPipelineBehavior<,>).FullName}' interface.",
                nameof(requestPipelineBehaviorType));
        }

        // Ensure the type is not abstract and has a public constructor
        if (requestPipelineBehaviorType.IsAbstract)
        {
            throw new ArgumentException(
                $"Type '{requestPipelineBehaviorType.FullName}' cannot be abstract.",
                nameof(requestPipelineBehaviorType));
        }

        if (requestPipelineBehaviorType.IsInterface)
        {
            throw new ArgumentException(
                $"Type '{requestPipelineBehaviorType.FullName}' cannot be an interface.",
                nameof(requestPipelineBehaviorType));
        }
    }
}
