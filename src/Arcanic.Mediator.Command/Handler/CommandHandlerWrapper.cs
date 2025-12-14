using Arcanic.Mediator.Command.Abstractions;

namespace Arcanic.Mediator.Command.Handler;

/// <summary>
///     Abstract base class for command handler wrappers.
///     Provides an abstract method for handling commands of type <see cref="ICommand"/>.
/// </summary>
public abstract class CommandHandlerWrapper : CommandHandlerWrapperBase
{
    /// <summary>
    ///     Handles the specified command request using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request">The command request to handle.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with a result of type <see cref="Unit"/>.
    /// </returns>
    public abstract Task<Unit> Handle(ICommand request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}