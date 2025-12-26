using Arcanic.Mediator.Abstractions.Handler.Post;

namespace Arcanic.Mediator.Command.Abstractions.Handler;

/// <summary>
/// Defines a post-handler that executes after the main command handler.
/// Post-handlers are useful for implementing cross-cutting concerns such as logging,
/// caching, cleanup operations, or other follow-up activities.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandPostHandler<TCommand> : IAsyncMessagePostHandler<TCommand> where TCommand : notnull;