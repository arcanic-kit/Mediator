// using Arcanic.Mediator.Query.Abstractions;
// using Arcanic.Mediator.Query.Abstractions.Handler;
//
// namespace Arcanic.Mediator.Query.Tests.Data;
//
// // Simple Query
// public class SimpleQuery : IQuery<SimpleQueryResponse>
// {
//     public int Value { get; init; }
// }
//
// public class SimpleQueryResponse
// {
//     public int Result { get; init; }
//     public string Message { get; init; } = string.Empty;
// }
//
// public class SimpleQueryHandler : IQueryHandler<SimpleQuery, SimpleQueryResponse>
// {
//     public Task<SimpleQueryResponse> HandleAsync(SimpleQuery request, CancellationToken cancellationToken = default)
//     {
//         return Task.FromResult(new SimpleQueryResponse
//         {
//             Result = request.Value * 2,
//             Message = $"Processed {request.Value}"
//         });
//     }
// }
//
// // Complex Query
// public class ComplexQuery : IQuery<ComplexQueryResponse>
// {
//     public string Name { get; init; } = string.Empty;
//     public int Count { get; init; }
// }
//
// public class ComplexQueryResponse
// {
//     public List<ComplexItem> Items { get; init; } = new();
//     public int TotalCount { get; init; }
// }
//
// public class ComplexItem
// {
//     public string Name { get; init; } = string.Empty;
//     public int Index { get; init; }
// }
//
// public class ComplexQueryHandler : IQueryHandler<ComplexQuery, ComplexQueryResponse>
// {
//     public Task<ComplexQueryResponse> HandleAsync(ComplexQuery request, CancellationToken cancellationToken = default)
//     {
//         var items = Enumerable.Range(0, request.Count)
//             .Select(i => new ComplexItem
//             {
//                 Name = $"{request.Name}_{i}",
//                 Index = i
//             })
//             .ToList();
//
//         return Task.FromResult(new ComplexQueryResponse
//         {
//             Items = items,
//             TotalCount = items.Count
//         });
//     }
// }
//
// // Query with Dependencies
// public class QueryWithDependencies : IQuery<QueryWithDependenciesResponse>
// {
//     public string Data { get; init; } = string.Empty;
// }
//
// public class QueryWithDependenciesResponse
// {
//     public string ProcessedData { get; init; } = string.Empty;
//     public string ServiceName { get; init; } = string.Empty;
// }
//
// public interface ITestDependency
// {
//     string Process(string data);
//     string GetName();
// }
//
// public class TestDependency : ITestDependency
// {
//     public string Process(string data) => $"Processed: {data}";
//     public string GetName() => "TestDependency";
// }
//
// public class QueryWithDependenciesHandler : IQueryHandler<QueryWithDependencies, QueryWithDependenciesResponse>
// {
//     private readonly ITestDependency _dependency;
//
//     public QueryWithDependenciesHandler(ITestDependency dependency)
//     {
//         _dependency = dependency;
//     }
//
//     public Task<QueryWithDependenciesResponse> HandleAsync(QueryWithDependencies request, CancellationToken cancellationToken = default)
//     {
//         return Task.FromResult(new QueryWithDependenciesResponse
//         {
//             ProcessedData = _dependency.Process(request.Data),
//             ServiceName = _dependency.GetName()
//         });
//     }
// }
//
// // Async Query
// public class AsyncQuery : IQuery<AsyncQueryResponse>
// {
//     public int DelayMs { get; init; }
// }
//
// public class AsyncQueryResponse
// {
//     public DateTime ExecutedAt { get; init; }
//     public int DelayMs { get; init; }
// }
//
// public class AsyncQueryHandler : IQueryHandler<AsyncQuery, AsyncQueryResponse>
// {
//     public async Task<AsyncQueryResponse> HandleAsync(AsyncQuery request, CancellationToken cancellationToken = default)
//     {
//         await Task.Delay(request.DelayMs, cancellationToken);
//         return new AsyncQueryResponse
//         {
//             ExecutedAt = DateTime.UtcNow,
//             DelayMs = request.DelayMs
//         };
//     }
// }
//
// // Query that throws exception
// public class ExceptionQuery : IQuery<ExceptionQueryResponse>
// {
//     public string ErrorMessage { get; init; } = string.Empty;
// }
//
// public class ExceptionQueryResponse
// {
//     public bool Success { get; init; }
// }
//
// public class ExceptionQueryHandler : IQueryHandler<ExceptionQuery, ExceptionQueryResponse>
// {
//     public Task<ExceptionQueryResponse> HandleAsync(ExceptionQuery request, CancellationToken cancellationToken = default)
//     {
//         throw new InvalidOperationException(request.ErrorMessage);
//     }
// }
