// using Arcanic.Mediator.Query.Abstractions.Handler;
//
// namespace Arcanic.Mediator.Query.Tests.Data;
//
// // Basic Post-Handler
// public class TestPostHandler : IQueryPostHandler<SimpleQuery>
// {
//     public bool WasExecuted { get; private set; }
//     public SimpleQuery? ReceivedQuery { get; private set; }
//
//     public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
//     {
//         WasExecuted = true;
//         ReceivedQuery = query;
//         return Task.CompletedTask;
//     }
// }
//
// //// Order Tracking Post-Handler
// //public class OrderTrackingPostHandler : IQueryPostHandler<SimpleQuery>
// //{
// //    private readonly List<string> _executionOrder;
//
// //    public OrderTrackingPostHandler(List<string> executionOrder)
// //    {
// //        _executionOrder = executionOrder;
// //    }
//
// //    public string Name { get; init; } = string.Empty;
//
// //    public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
// //    {
// //        _executionOrder.Add($"PostHandler_{Name}");
// //        return Task.CompletedTask;
// //    }
// //}
//
// // Response Capturing Post-Handler (Note: Post handlers don't receive response, but we can track execution)
// public class ResponseCapturingPostHandler : IQueryPostHandler<SimpleQuery>
// {
//     public bool WasExecuted { get; private set; }
//     public DateTime ExecutedAt { get; private set; }
//
//     public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
//     {
//         WasExecuted = true;
//         ExecutedAt = DateTime.UtcNow;
//         return Task.CompletedTask;
//     }
// }
//
// // Exception Throwing Post-Handler
// public class ExceptionThrowingPostHandler : IQueryPostHandler<SimpleQuery>
// {
//     public string ErrorMessage { get; init; } = "Post-handler error";
//
//     public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
//     {
//         throw new InvalidOperationException(ErrorMessage);
//     }
// }
//
// // Post-Handler for Complex Query
// public class ComplexQueryPostHandler : IQueryPostHandler<ComplexQuery>
// {
//     public bool WasExecuted { get; private set; }
//
//     public Task HandleAsync(ComplexQuery query, CancellationToken cancellationToken = default)
//     {
//         WasExecuted = true;
//         return Task.CompletedTask;
//     }
// }
//
// // Post-Handler with Dependencies
// public class DependencyPostHandler : IQueryPostHandler<SimpleQuery>
// {
//     private readonly ITestDependency _dependency;
//
//     public DependencyPostHandler(ITestDependency dependency)
//     {
//         _dependency = dependency;
//     }
//
//     public bool WasExecuted { get; private set; }
//
//     public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
//     {
//         WasExecuted = true;
//         _ = _dependency.GetName(); // Use dependency
//         return Task.CompletedTask;
//     }
// }
