using Arcanic.Mediator.Messaging.Abstractions.Registry;

namespace Arcanic.Mediator.Messaging.Registry;

/// <summary>
/// Provides static access to a singleton instance of the message registry.
/// This accessor ensures thread-safe lazy initialization of the registry and maintains
/// a single shared instance throughout the application lifecycle.
/// </summary>
public static class MessageRegistryAccessor
{
    /// <summary>
    /// The lazy-initialized singleton instance of the message registry.
    /// Uses ExecutionAndPublication thread safety mode to ensure only one instance
    /// is created even under concurrent access scenarios.
    /// </summary>
    private static readonly Lazy<IMessageRegistry> LazyInstance = new(() => new MessageRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Gets the singleton instance of the message registry.
    /// This property provides thread-safe access to the shared registry instance
    /// that maintains all message type and handler registrations.
    /// </summary>
    /// <value>The singleton message registry instance.</value>
    public static IMessageRegistry Instance => LazyInstance.Value;
}