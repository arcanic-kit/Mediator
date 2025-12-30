namespace Arcanic.Mediator.Abstractions;

/// <summary>
/// Represents the base contract for commands and queries in the mediator pattern.
/// This interface serves as a marker interface to identify types that can be processed by the mediator.
/// </summary>
/// <remarks>
/// Implement this interface on your message types (commands, queries) to enable them
/// to be handled by the mediator infrastructure. This interface provides type safety and
/// ensures that only valid message types can be processed through the mediator pipeline.
/// </remarks>
public interface IRequest : IMessage { }