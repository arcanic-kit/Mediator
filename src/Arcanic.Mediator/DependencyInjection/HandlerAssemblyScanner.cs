using System.Reflection;
using Arcanic.Mediator.Abstractions.DependencyInjection;

namespace Arcanic.Mediator.DependencyInjection;

/// <summary>
/// Discovers handler implementations in an assembly and registers them.
/// Shared by the command, query and event modules.
/// </summary>
internal static class HandlerAssemblyScanner
{
    /// <summary>
    /// Registers every concrete, closed class in <paramref name="assembly"/> against each handler interface it implements.
    /// Registration is idempotent: scanning the same assembly twice does not duplicate handlers.
    /// </summary>
    /// <param name="serviceRegistrar">The registrar to add handlers to.</param>
    /// <param name="assembly">The assembly to scan.</param>
    /// <param name="isHandlerInterface">Returns <c>true</c> for the module handler interfaces (main, pre and post).</param>
    /// <param name="isMessageInterface">Returns <c>true</c> for the module message marker interfaces.</param>
    /// <param name="messageInterfaceName">Display name of the message interface, used in error messages.</param>
    /// <exception cref="InvalidOperationException">Thrown when a handled message type does not implement the module message interface.</exception>
    public static IServiceRegistrar RegisterHandlers(
        IServiceRegistrar serviceRegistrar,
        Assembly assembly,
        Func<Type, bool> isHandlerInterface,
        Func<Type, bool> isMessageInterface,
        string messageInterfaceName)
    {
        ArgumentNullException.ThrowIfNull(serviceRegistrar);
        ArgumentNullException.ThrowIfNull(assembly);

        // Open generic handler classes are skipped: they cannot be registered against a closed handler interface.
        var handlerTypes = GetLoadableTypes(assembly)
            .Where(type => type is { IsAbstract: false, IsInterface: false, IsClass: true, ContainsGenericParameters: false });

        foreach (var handlerType in handlerTypes)
        {
            foreach (var handlerInterface in handlerType.GetInterfaces().Where(isHandlerInterface))
            {
                var messageType = handlerInterface.GetGenericArguments()[0];

                if (!messageType.GetInterfaces().Any(isMessageInterface))
                {
                    throw new InvalidOperationException(
                        $"Message type '{messageType.FullName}' handled by '{handlerType.FullName}' must implement {messageInterfaceName} interface.");
                }

                serviceRegistrar.TryRegister(handlerInterface, handlerType);
            }
        }

        return serviceRegistrar;
    }

    /// <summary>
    /// Returns the types of an assembly, skipping types that cannot be loaded (for example when an
    /// optional dependency is missing) instead of failing the whole scan.
    /// </summary>
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.OfType<Type>();
        }
    }
}
