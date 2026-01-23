// using Arcanic.Mediator.Query.Abstractions.Handler;
//
// namespace Arcanic.Mediator.Query.Tests.Data;
//
// // Basic Pre-Handler
// public class TestPreHandler : IQueryPreHandler<SimpleQuery>
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
// // Order Tracking Pre-Handler
// //public class OrderTrackingPreHandler : IQueryPreHandler<SimpleQuery>
// //{
// //    private readonly List<string> _executionOrder;
//
// //    public OrderTrackingPreHandler(List<string> executionOrder)
// //    {
// //        _executionOrder = executionOrder;
// //    }
//
// //    public string Name { get; init; } = string.Empty;
//
// //    public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
// //    {
// //        _executionOrder.Add($"PreHandler_{Name}");
// //        return Task.CompletedTask;
// //    }
// //}
//
// // Exception Throwing Pre-Handler
// public class ExceptionThrowingPreHandler : IQueryPreHandler<SimpleQuery>
// {
//     public string ErrorMessage { get; init; } = "Pre-handler error";
//
//     public Task HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
//     {
//         throw new InvalidOperationException(ErrorMessage);
//     }
// }
//
// // Pre-Handler for Complex Query
// public class ComplexQueryPreHandler : IQueryPreHandler<ComplexQuery>
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
// // Pre-Handler with Dependencies
// public class DependencyPreHandler : IQueryPreHandler<SimpleQuery>
// {
//     private readonly ITestDependency _dependency;
//
//     public DependencyPreHandler(ITestDependency dependency)
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
