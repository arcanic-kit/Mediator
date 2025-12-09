using Arcanic.Mediator.Benchmarks.Commands.CreateUser;
using Arcanic.Mediator.Benchmarks.Commands.DeleteUser;
using Arcanic.Mediator.Benchmarks.Commands.UpdateUser;
using Arcanic.Mediator.Command.Abstractions;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Benchmarks.Commands;

[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class CommandBenchmarks
{
    private IServiceProvider _serviceProvider = default!;
    private ICommandMediator _commandMediator = default!;

    [GlobalSetup]
    public void Setup()
    {
        var arcanicServices = new ServiceCollection();
        arcanicServices.AddArcanicMediator();
        _serviceProvider = arcanicServices.BuildServiceProvider();
        _commandMediator = _serviceProvider.GetRequiredService<ICommandMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_serviceProvider is IDisposable arcanicDisposable)
            arcanicDisposable.Dispose();
    }

    [Benchmark]
    [BenchmarkCategory("CommandWithResult")]
    public async Task<CreateUserCommandResponse> CreateUser()
    {
        var command = new CreateUserCommand
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        return await _commandMediator.SendAsync(command);
    }

    [Benchmark]
    [BenchmarkCategory("VoidCommand")]
    public async Task UpdateUser()
    {
        var command = new UpdateUserCommand
        {
            Id = 1,
            Name = "Jane Doe",
            Email = "jane@example.com"
        };
        await _commandMediator.SendAsync(command);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexCommand")]
    public async Task DeleteUser()
    {
        var command = new DeleteUserCommand
        {
            Id = 1,
            Reason = "Account closed by user request"
        };
        await _commandMediator.SendAsync(command);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task MultipleCommands(int commandCount)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= commandCount; i++)
        {
            var command = new UpdateUserCommand
            {
                Id = i,
                Name = $"User {i}",
                Email = $"user{i}@example.com"
            };
            tasks.Add(_commandMediator.SendAsync(command).AsTask());
        }
        await Task.WhenAll(tasks);
    }
}