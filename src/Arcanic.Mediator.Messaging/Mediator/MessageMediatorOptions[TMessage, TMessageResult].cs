using Arcanic.Mediator.Messaging.Abstractions.Mediator;

namespace Arcanic.Mediator.Messaging.Mediator;

/// <summary>
/// Provides a concrete implementation of message mediator options that configure how messages are processed
/// through the mediator framework. This sealed class encapsulates the strategy and cancellation token
/// required for message mediation operations.
/// </summary>
/// <typeparam name="TMessage">The type of message being mediated. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result expected from the message mediation process.</typeparam>
public sealed class MessageMediatorOptions<TMessage, TMessageResult> : IMessageMediatorOptions<TMessage, TMessageResult> where TMessage : notnull
{
    /// <summary>
    /// Gets or initializes the strategy that defines how the message will be processed through the mediator.
    /// The strategy determines the execution flow and handler coordination for the message.
    /// This property is required and must be set during object initialization.
    /// </summary>
    /// <value>The mediation strategy that will orchestrate the message processing.</value>
    public required IMessageMediatorStrategy<TMessage, TMessageResult> Strategy { get; init; }

    /// <summary>
    /// Gets or initializes the cancellation token that can be used to cancel the message mediation process.
    /// This token allows for cooperative cancellation of long-running or resource-intensive operations.
    /// This property is required and defaults to <see cref="CancellationToken.None"/> if not explicitly set.
    /// </summary>
    /// <value>The cancellation token for controlling the mediation lifecycle.</value>
    public required CancellationToken CancellationToken { get; init; } = CancellationToken.None;
}
