using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Abstractions.Mediator;

/// <summary>
/// Defines a strategy for mediating messages through the mediator framework.
/// Strategies encapsulate the logic for how messages are processed, coordinating between
/// the message, available handlers, and the mediation context to produce results.
/// </summary>
/// <typeparam name="TMessage">The type of message that this strategy can mediate. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this strategy produces from message mediation.</typeparam>
public interface IMessageMediatorStrategy<in TMessage, out TMessageResult>
    where TMessage : notnull
{
    /// <summary>
    /// Mediates the processing of a message using the provided handlers and context.
    /// This method implements the core strategy logic for how messages are routed to handlers
    /// and how their results are processed and returned.
    /// </summary>
    /// <param name="message">The message instance to be mediated and processed.</param>
    /// <param name="handlerProvider">The provider that supplies access to registered main handlers for message processing.</param>
    /// <param name="context">The mediation context containing cancellation token and result state information.</param>
    /// <returns>The result of mediating the message through the strategy's processing logic.</returns>
    TMessageResult Mediate(TMessage message, IMessageMediatorHandlerProvider handlerProvider, IMessageMediatorContext context);
}