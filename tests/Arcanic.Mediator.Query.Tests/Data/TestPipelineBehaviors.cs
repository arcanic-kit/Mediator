// using Arcanic.Mediator.Abstractions.Pipeline;
// using Arcanic.Mediator.Query.Abstractions;
// using Arcanic.Mediator.Query.Abstractions.Pipeline;
//
// namespace Arcanic.Mediator.Query.Tests.Data;
//
// public class TestQueryPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
//     where TQuery : IQuery<TResponse>
// {
//     public bool WasExecuted { get; private set; }
//     public TQuery? ReceivedQuery { get; private set; }
//
//     public async Task<TResponse> HandleAsync(TQuery message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
//     {
//         WasExecuted = true;
//         ReceivedQuery = message;
//         return await next(cancellationToken);
//     }
// }
//
// // Logging Pipeline Behavior
// public class LoggingPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
//     where TQuery : IQuery<TResponse>
// {
//     private readonly List<string> _logEntries;
//
//     public LoggingPipelineBehavior(List<string> logEntries)
//     {
//         _logEntries = logEntries;
//     }
//
//     public async Task<TResponse> HandleAsync(TQuery message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
//     {
//         _logEntries.Add($"Before: {typeof(TQuery).Name}");
//         var result = await next(cancellationToken);
//         _logEntries.Add($"After: {typeof(TQuery).Name}");
//         return result;
//     }
// }
//
// // Response Modifying Pipeline Behavior
// public class ResponseModifyingPipelineBehavior : IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>
// {
//     public string Modification { get; init; } = "Modified";
//
//     public async Task<SimpleQueryResponse> HandleAsync(SimpleQuery message, PipelineDelegate<SimpleQueryResponse> next, CancellationToken cancellationToken = default)
//     {
//         var response = await next(cancellationToken);
//         return new SimpleQueryResponse
//         {
//             Result = response.Result,
//             Message = $"{response.Message} - {Modification}"
//         };
//     }
// }
//
// // Order Tracking Pipeline Behavior
// public class OrderTrackingPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
//     where TQuery : IQuery<TResponse>
// {
//     private readonly List<string> _executionOrder;
//
//     public OrderTrackingPipelineBehavior(List<string> executionOrder)
//     {
//         _executionOrder = executionOrder;
//     }
//
//     public string Name { get; init; } = string.Empty;
//
//     public async Task<TResponse> HandleAsync(TQuery message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
//     {
//         _executionOrder.Add($"PipelineBehavior_{Name}_Before");
//         var result = await next(cancellationToken);
//         _executionOrder.Add($"PipelineBehavior_{Name}_After");
//         return result;
//     }
// }
//
// // Short-Circuit Pipeline Behavior
// public class ShortCircuitPipelineBehavior : IQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>
// {
//     public SimpleQueryResponse? ShortCircuitResponse { get; init; }
//
//     public Task<SimpleQueryResponse> HandleAsync(SimpleQuery message, PipelineDelegate<SimpleQueryResponse> next, CancellationToken cancellationToken = default)
//     {
//         if (ShortCircuitResponse != null)
//         {
//             return Task.FromResult(ShortCircuitResponse);
//         }
//         return next(cancellationToken);
//     }
// }
//
// // Exception Throwing Pipeline Behavior
// public class ExceptionThrowingPipelineBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
//     where TQuery : IQuery<TResponse>
// {
//     public string ErrorMessage { get; init; } = "Pipeline behavior error";
//
//     public Task<TResponse> HandleAsync(TQuery message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default)
//     {
//         throw new InvalidOperationException(ErrorMessage);
//     }
// }
