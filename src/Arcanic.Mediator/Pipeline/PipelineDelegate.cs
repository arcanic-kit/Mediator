namespace Arcanic.Mediator.Pipeline;

/// <summary>
/// Represents a delegate that can be invoked to continue the message processing pipeline.
/// This delegate encapsulates the remaining behaviors and the final handler execution.
/// </summary>
/// <typeparam name="TResponse">The type of result expected from the pipeline execution.</typeparam>
/// <returns>A task representing the asynchronous execution of the remaining pipeline, containing the result.</returns>
public delegate Task<TResponse> PipelineDelegate<TResponse>(CancellationToken t = default);