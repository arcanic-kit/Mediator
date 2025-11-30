using Arcanic.Mediator.Messaging.Abstractions.Handler.Post;

namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Defines a post-handler that executes after the main command handler for commands that return a result.
/// Post-handlers are useful for implementing cross-cutting concerns such as logging,
/// caching, cleanup operations, or other follow-up activities.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand{TCommandResult}"/>.</typeparam>
/// <typeparam name="TCommandResult">The type of result returned by the command.</typeparam>
public interface ICommandPostHandler<TCommand, TCommandResult> : IAsyncMessagePostHandler<TCommand, TCommandResult> where TCommand : ICommand<TCommandResult>;