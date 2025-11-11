namespace Arcanic.Mediator.Command.Abstractions;

public interface ICommandMediator
{
    Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

    Task<TCommandResult> SendAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default);
}