using Arcanic.Mediator.Messaging.Abstractions.Handler.Pre;

namespace Arcanic.Mediator.Command.Abstractions;

/// <summary>
/// Defines a pre-handler that executes before the main command handler.
/// Pre-handlers are useful for implementing cross-cutting concerns such as validation,
/// authentication, logging, or other preparatory operations.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle, which must implement <see cref="ICommand"/>.</typeparam>
public interface ICommandPreHandler<TCommand> : IAsyncMessagePreHandler<TCommand> where TCommand : ICommand;