using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.PipelineBehaviors;

/// <summary>
/// An example command pipeline behavior that demonstrates logging capabilities in the command processing pipeline.
/// This behavior logs command execution information including the command name and a unique correlation ID.
/// </summary>
/// <typeparam name="TCommand">The type of command being processed. Must implement <see cref="ICommand"/>.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the command handler.</typeparam>
public class ExampleCommandPipelineBehavior<TCommand, TResponse> : ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand
{
    /// <summary>
    /// The logger instance used to write command execution information.
    /// </summary>
    private readonly ILogger<ExampleCommandPipelineBehavior<TCommand, TResponse>> _logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleCommandPipelineBehavior{TCommand, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used for writing log information.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public ExampleCommandPipelineBehavior(ILogger<ExampleCommandPipelineBehavior<TCommand, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Handles the command execution by logging command information and then calling the next behavior in the pipeline.
    /// This method generates a correlation ID for tracking and logs the command execution start.
    /// </summary>
    /// <param name="command">The command instance being processed.</param>
    /// <param name="next">The next delegate in the pipeline to invoke.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response from the command handler.</returns>
    public async Task<TResponse> HandleAsync(TCommand command, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;
        var correlationId = Guid.NewGuid();
        
        _logger.LogInformation("[COMMAND BEHAVIOR] Starting execution of {CommandName} with correlation ID {CorrelationId}", commandName, correlationId);
        
        return await next(cancellationToken);
    }
}