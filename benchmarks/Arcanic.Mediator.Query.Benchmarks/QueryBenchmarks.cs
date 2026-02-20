using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcanic.Mediator.Query.Benchmarks.GetUser;
using Arcanic.Mediator.Query.Benchmarks.SearchUsers;
using Arcanic.Mediator.Request.Abstractions;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Query.Benchmarks;

/// <summary>
/// Provides performance benchmarks for query processing through the Arcanic Mediator.
/// Measures execution time and memory allocation for various query scenarios.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class QueryBenchmarks
{
    private IServiceProvider _serviceProvider = default!;
    private IQuerySender _querySender = default!;

    /// <summary>
    /// Initializes the dependency injection container and query mediator for benchmarking.
    /// Configures Arcanic Mediator services and builds the service provider.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        var arcanicServices = new ServiceCollection();
        arcanicServices.AddArcanicQueryMediator();
        _serviceProvider = arcanicServices.BuildServiceProvider();
        _querySender = _serviceProvider.GetRequiredService<IQuerySender>();
    }

    /// <summary>
    /// Disposes of the service provider and releases allocated resources after benchmarking.
    /// </summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        if (_serviceProvider is IDisposable arcanicDisposable)
            arcanicDisposable.Dispose();
    }

    /// <summary>
    /// Benchmarks the execution of a simple user lookup query by ID.
    /// Measures performance for basic single-entity retrieval operations.
    /// </summary>
    /// <returns>The response containing the requested user information.</returns>
    [Benchmark]
    [BenchmarkCategory("SimpleQuery")]
    public async Task<GetUserQueryResponse> GetUser()
    {
        var query = new GetUserQuery { Id = 1 };
        return await _querySender.SendAsync(query);
    }

    /// <summary>
    /// Benchmarks the execution of a complex search query with multiple criteria and pagination.
    /// Measures performance for advanced query operations involving filtering and result pagination.
    /// </summary>
    /// <returns>The response containing the search results and pagination information.</returns>
    [Benchmark]
    [BenchmarkCategory("ComplexQuery")]
    public async Task<SearchUsersQueryResponse> SearchUsers()
    {
        var query = new SearchUsersQuery
        {
            Name = "John",
            Email = "john@example.com",
            Page = 1,
            PageSize = 10
        };
        return await _querySender.SendAsync(query);
    }

    /// <summary>
    /// Benchmarks high-throughput query processing by executing multiple queries concurrently.
    /// Measures performance under load with parallel query execution scenarios.
    /// </summary>
    /// <param name="queryCount">The number of concurrent queries to execute.</param>
    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task MultipleQueries(int queryCount)
    {
        var tasks = new List<Task<GetUserQueryResponse>>();
        for (int i = 1; i <= queryCount; i++)
        {
            var query = new GetUserQuery { Id = i };
            // Convert ValueTask to Task using AsTask()
            tasks.Add(_querySender.SendAsync(query));
        }
        await Task.WhenAll(tasks);
    }
}