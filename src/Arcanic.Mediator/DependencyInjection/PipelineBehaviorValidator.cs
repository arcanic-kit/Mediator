namespace Arcanic.Mediator.DependencyInjection;

/// <summary>
/// Validates pipeline behavior types before they are registered.
/// Shared by the generic, request, command, query and event behavior registration methods.
/// </summary>
internal static class PipelineBehaviorValidator
{
    /// <summary>
    /// Ensures <paramref name="behaviorType"/> is a concrete type implementing <paramref name="openBehaviorInterface"/>.
    /// </summary>
    /// <param name="behaviorType">The behavior type to validate, typically an open generic such as <c>typeof(MyBehavior&lt;,&gt;)</c>.</param>
    /// <param name="openBehaviorInterface">The open generic behavior interface the type must implement.</param>
    /// <param name="parameterName">The calling method's parameter name, used in thrown exceptions.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="behaviorType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when the type does not implement the interface, or is abstract or an interface.</exception>
    public static void Validate(Type behaviorType, Type openBehaviorInterface, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(behaviorType, parameterName);

        var implementsInterface = behaviorType
            .GetInterfaces()
            .Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == openBehaviorInterface);

        if (!implementsInterface)
        {
            throw new ArgumentException(
                $"Type '{behaviorType.FullName}' must implement '{openBehaviorInterface.FullName}' interface.",
                parameterName);
        }

        if (behaviorType.IsInterface)
        {
            throw new ArgumentException(
                $"Type '{behaviorType.FullName}' cannot be an interface.",
                parameterName);
        }

        if (behaviorType.IsAbstract)
        {
            throw new ArgumentException(
                $"Type '{behaviorType.FullName}' cannot be abstract.",
                parameterName);
        }
    }
}
