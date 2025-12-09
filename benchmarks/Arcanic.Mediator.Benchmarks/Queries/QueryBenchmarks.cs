using Arcanic.Mediator.Benchmarks.Queries.GetUser;
using Arcanic.Mediator.Benchmarks.Queries.SearchUsers;
using Arcanic.Mediator.Query.Abstractions;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Benchmarks.Queries;

[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class QueryBenchmarks
{
    private IServiceProvider _serviceProvider = default!;
    private IQueryMediator _queryMediator = default!;

    [GlobalSetup]
    public void Setup()
    {
        var arcanicServices = new ServiceCollection();
        arcanicServices.AddArcanicMediator();
        _serviceProvider = arcanicServices.BuildServiceProvider();
        _queryMediator = _serviceProvider.GetRequiredService<IQueryMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_serviceProvider is IDisposable arcanicDisposable)
            arcanicDisposable.Dispose();
    }

    [Benchmark]
    [BenchmarkCategory("SimpleQuery")]
    public async Task<GetUserQueryResponse> GetUser()
    {
        var query = new GetUserQuery { Id = 1 };
        return await _queryMediator.SendAsync(query);
    }

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
        return await _queryMediator.SendAsync(query);
    }

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
            tasks.Add(_queryMediator.SendAsync(query).AsTask());
        }
        await Task.WhenAll(tasks);
    }
}