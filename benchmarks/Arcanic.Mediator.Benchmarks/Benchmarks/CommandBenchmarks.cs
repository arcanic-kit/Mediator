using Arcanic.Mediator.Benchmarks.Arcanic.Commands;
using Arcanic.Mediator.Benchmarks.Configuration;
using Arcanic.Mediator.Benchmarks.MediatR.Commands;
using Arcanic.Mediator.Benchmarks.Models;
using Arcanic.Mediator.Command.Abstractions;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks comparing command performance between MediatR and Arcanic Mediator
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class CommandBenchmarks
{
    private IServiceProvider _mediatRServiceProvider = default!;
    private IServiceProvider _arcanicServiceProvider = default!;

    private IMediator _mediatr = default!;
    private ICommandMediator _arcanicCommandMediator = default!;

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
        _arcanicCommandMediator = _arcanicServiceProvider.GetRequiredService<ICommandMediator>();
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
    [BenchmarkCategory("CommandWithResult")]
    public async Task<CreateUserCommandResult> MediatR_CreateUser()
    {
        var request = new CreateUserRequest("John Doe", "john@example.com");
        return await _mediatr.Send(request);
    }

    [Benchmark]
    [BenchmarkCategory("CommandWithResult")]
    public async Task<CreateUserCommandResult> Arcanic_CreateUser()
    {
        var command = new CreateUserArcanicCommand
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        return await _arcanicCommandMediator.SendAsync(command);
    }

    [Benchmark]
    [BenchmarkCategory("VoidCommand")]
    public async Task MediatR_UpdateUser()
    {
        var request = new UpdateUserRequest(1, "Jane Doe", "jane@example.com");
        await _mediatr.Send(request);
    }

    [Benchmark]
    [BenchmarkCategory("VoidCommand")]
    public async Task Arcanic_UpdateUser()
    {
        var command = new UpdateUserArcanicCommand
        {
            Id = 1,
            Name = "Jane Doe",
            Email = "jane@example.com"
        };
        await _arcanicCommandMediator.SendAsync(command);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexCommand")]
    public async Task MediatR_DeleteUser()
    {
        var request = new DeleteUserRequest(1, "Account closed by user request");
        await _mediatr.Send(request);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexCommand")]
    public async Task Arcanic_DeleteUser()
    {
        var command = new DeleteUserArcanicCommand
        {
            Id = 1,
            Reason = "Account closed by user request"
        };
        await _arcanicCommandMediator.SendAsync(command);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task MediatR_MultipleCommands(int commandCount)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= commandCount; i++)
        {
            var request = new UpdateUserRequest(i, $"User {i}", $"user{i}@example.com");
            tasks.Add(_mediatr.Send(request));
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task Arcanic_MultipleCommands(int commandCount)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= commandCount; i++)
        {
            var command = new UpdateUserArcanicCommand
            {
                Id = i,
                Name = $"User {i}",
                Email = $"user{i}@example.com"
            };
            tasks.Add(_arcanicCommandMediator.SendAsync(command));
        }
        await Task.WhenAll(tasks);
    }
}