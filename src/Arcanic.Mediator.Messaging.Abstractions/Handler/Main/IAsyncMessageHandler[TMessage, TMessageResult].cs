using Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

namespace Arcanic.Mediator.Messaging.Abstractions.Handler.Main;

/// <summary>
/// Defines an asynchronous message handler that processes messages and returns a specific result type.
/// This interface extends the base message handler to support asynchronous operations with typed results and cancellation support.
/// </summary>
/// <typeparam name="TMessage">The type of message that this handler can process. Must be a non-null reference type.</typeparam>
/// <typeparam name="TMessageResult">The type of result that this handler produces from message processing.</typeparam>
public interface IAsyncMessageHandler<TMessage, TMessageResult> : IMessageHandler<TMessage, Task<TMessageResult>> where TMessage : notnull
{
    /// <summary>
    /// Provides the synchronous interface implementation by delegating to the asynchronous handler method.
    /// This method retrieves the cancellation token from the current mediator context and passes it to the async handler.
    /// </summary>
    /// <param name="message">The message to be processed by the handler.</param>
    /// <returns>A task representing the asynchronous message processing operation that yields a typed result.</returns>
    Task<TMessageResult> IMessageHandler<TMessage, Task<TMessageResult>>.Handle(TMessage message)
    {
        return HandleAsync(message, MessageMediatorContextAccessor.Current.CancellationToken);
    }

    /// <summary>
    /// Asynchronously handles a message and returns a typed result with support for cancellation.
    /// This method contains the core business logic for processing the message asynchronously and producing a result.
    /// </summary>
    /// <param name="message">The message to be processed by the handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. Defaults to CancellationToken.None.</param>
    /// <returns>A task representing the asynchronous message processing operation that yields a typed result.</returns>
    Task<TMessageResult> HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}