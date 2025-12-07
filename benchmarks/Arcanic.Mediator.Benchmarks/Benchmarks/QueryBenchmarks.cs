using Arcanic.Mediator.Benchmarks.Arcanic.Queries;
using Arcanic.Mediator.Benchmarks.Configuration;
using Arcanic.Mediator.Benchmarks.MediatR.Queries;
using Arcanic.Mediator.Benchmarks.Models;
using Arcanic.Mediator.Query.Abstractions;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks comparing query performance between MediatR and Arcanic Mediator
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class QueryBenchmarks
{
    private IServiceProvider _mediatRServiceProvider = default!;
    private IServiceProvider _arcanicServiceProvider = default!;

    private IMediator _mediatr = default!;
    private IQueryMediator _arcanicQueryMediator = default!;

    [GlobalSetup]
    public void Setup()
    {
        // Setup MediatR
        var mediatRServices = new ServiceCollection();
        mediatRServices.AddMediatR();
        _mediatRServiceProvider = mediatRServices.BuildServiceProvider();
        _mediatr = _mediatRServiceProvider.GetRequiredService<IMediator>();

        // Setup Arcanic Mediator
        var arcanicServices = new ServiceCollection();
        arcanicServices.AddArcanicMediator();
        _arcanicServiceProvider = arcanicServices.BuildServiceProvider();
        _arcanicQueryMediator = _arcanicServiceProvider.GetRequiredService<IQueryMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_mediatRServiceProvider is IDisposable mediatRDisposable)
            mediatRDisposable.Dispose();
        
        if (_arcanicServiceProvider is IDisposable arcanicDisposable)
            arcanicDisposable.Dispose();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("SimpleQuery")]
    public async Task<GetUserQueryResult> MediatR_GetUser()
    {
        var request = new GetUserRequest(1);
        return await _mediatr.Send(request);
    }

    [Benchmark]
    [BenchmarkCategory("SimpleQuery")]
    public async Task<GetUserQueryResult> Arcanic_GetUser()
    {
        var query = new GetUserArcanicQuery { Id = 1 };
        return await _arcanicQueryMediator.SendAsync(query);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexQuery")]
    public async Task<SearchUsersQueryResult> MediatR_SearchUsers()
    {
        var request = new SearchUsersRequest("John", "john@example.com", 1, 10);
        return await _mediatr.Send(request);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexQuery")]
    public async Task<SearchUsersQueryResult> Arcanic_SearchUsers()
    {
        var query = new SearchUsersArcanicQuery
        {
            Name = "John",
            Email = "john@example.com",
            Page = 1,
            PageSize = 10
        };
        return await _arcanicQueryMediator.SendAsync(query);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task MediatR_MultipleQueries(int queryCount)
    {
        var tasks = new List<Task<GetUserQueryResult>>();
        for (int i = 1; i <= queryCount; i++)
        {
            var request = new GetUserRequest(i);
            tasks.Add(_mediatr.Send(request));
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task Arcanic_MultipleQueries(int queryCount)
    {
        var tasks = new List<Task<GetUserQueryResult>>();
        for (int i = 1; i <= queryCount; i++)
        {
            var query = new GetUserArcanicQuery { Id = i };
            // Convert ValueTask to Task using AsTask()
            tasks.Add(_arcanicQueryMediator.SendAsync(query).AsTask());
        }
        await Task.WhenAll(tasks);
    }
}