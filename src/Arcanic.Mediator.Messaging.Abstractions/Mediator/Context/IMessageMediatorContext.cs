namespace Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

/// <summary>
/// Defines the context for message mediation, providing access to cancellation control
/// and result state management during the mediation process.
/// </summary>
public interface IMessageMediatorContext
{
    /// <summary>
    /// Gets the cancellation token that can be used to cancel the mediation process.
    /// This token allows for cooperative cancellation of message processing operations.
    /// </summary>
    /// <value>The cancellation token for controlling the mediation lifecycle.</value>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets the result of the message mediation process.
    /// This property allows strategies and handlers to store and retrieve the mediation result
    /// as an untyped object that can be cast to the appropriate result type.
    /// </summary>
    /// <value>The result object from message processing, or null if no result has been set.</value>
    object? MessageResult { get; set; }
}
