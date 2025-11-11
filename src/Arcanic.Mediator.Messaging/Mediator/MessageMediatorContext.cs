using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Mediator;

/// <summary>
/// Provides a concrete implementation of the message mediator context that maintains state
/// during message mediation operations. This class encapsulates the cancellation token
/// and result storage for the mediation process.
/// </summary>
public class MessageMediatorContext : IMessageMediatorContext
{
    /// <summary>
    /// Gets the cancellation token that can be used to cancel the mediation process.
    /// This token allows for cooperative cancellation of message processing operations.
    /// </summary>
    /// <value>The cancellation token for controlling the mediation lifecycle.</value>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets the result of the message mediation process.
    /// This property allows strategies and handlers to store and retrieve the mediation result
    /// as an untyped object that can be cast to the appropriate result type.
    /// </summary>
    /// <value>The result object from message processing, or null if no result has been set.</value>
    public object? MessageResult { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageMediatorContext"/> class.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the mediation process.</param>
    public MessageMediatorContext(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }
}
