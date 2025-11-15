namespace Arcanic.Mediator.Messaging.Abstractions.Mediator;

/// <summary>
/// Defines configuration options for message mediation, specifying the strategy and cancellation token
/// to be used when processing messages through the mediator framework.
/// </summary>
/// <typeparam name="TMessage">The type of message being mediated. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result expected from the message mediation process.</typeparam>
public interface IMessageMediatorOptions<TMessage, TMessageResult> where TMessage : notnull
{
    /// <summary>
    /// Gets or initializes the strategy that defines how the message will be processed through the mediator.
    /// The strategy determines the execution flow and handler coordination for the message.
    /// </summary>
    /// <value>The mediation strategy that will orchestrate the message processing.</value>
    IMessageMediatorStrategy<TMessage, TMessageResult> Strategy { get; init; }

    /// <summary>
    /// Gets or initializes the cancellation token that can be used to cancel the message mediation process.
    /// This token allows for cooperative cancellation of long-running or resource-intensive operations.
    /// </summary>
    /// <value>The cancellation token for controlling the mediation lifecycle.</value>
    CancellationToken CancellationToken { get; init; }
}
