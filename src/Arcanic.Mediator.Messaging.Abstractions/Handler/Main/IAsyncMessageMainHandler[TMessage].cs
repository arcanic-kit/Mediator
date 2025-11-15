using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

/// <summary>
/// Defines an asynchronous main message handler that processes messages without returning a specific result.
/// This interface extends the base message handler to support asynchronous operations with cancellation support.
/// </summary>
/// <typeparam name="TMessage">The type of message that this handler can process. Must be a non-null reference type.</typeparam>
public interface IAsyncMessageMainHandler<TMessage> : IMessageHandler<TMessage, Task> where TMessage : notnull
{
    /// <summary>
    /// Provides the synchronous interface implementation by delegating to the asynchronous handler method.
    /// This method retrieves the cancellation token from the current mediator context and passes it to the async handler.
    /// </summary>
    /// <param name="message">The message to be processed by the handler.</param>
    /// <returns>A task representing the asynchronous message processing operation.</returns>
    Task IMessageHandler<TMessage, Task>.Handle(TMessage message)
    {
        return HandleAsync(message, MessageMediatorContextAccessor.Current.CancellationToken);
    }

    /// <summary>
    /// Asynchronously handles a message with support for cancellation.
    /// This method contains the core business logic for processing the message asynchronously.
    /// </summary>
    /// <param name="message">The message to be processed by the handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. Defaults to CancellationToken.None.</param>
    /// <returns>A task representing the asynchronous message processing operation.</returns>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}