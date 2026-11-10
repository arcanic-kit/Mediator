namespace Arcanic.Mediator.Tests.Shared;

/// <summary>
/// Records the types (handlers, pipeline behaviors) that executed during a test, in execution order.
/// Shared by all test projects through tests/Directory.Build.props.
/// </summary>
public class ExecutedTypeTracker
{
    public List<Type> ExecutedTypes { get; } = new();
}
