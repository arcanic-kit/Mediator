using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Benchmarks.CreateUser;
using Arcanic.Mediator.Command.Benchmarks.DeleteUser;
using Arcanic.Mediator.Command.Benchmarks.UpdateUser;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Command.Benchmarks;

/// <summary>
/// Provides performance benchmarks for command processing through the Arcanic Mediator.
/// Measures execution time and memory allocation for various command scenarios.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class CommandBenchmarks
{
    private IServiceProvider _serviceProvider = default!;
    private ICommandMediator _commandMediator = default!;

    /// <summary>
    /// Initializes the dependency injection container and command mediator for benchmarking.
    /// Configures Arcanic Mediator services and builds the service provider.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        var arcanicServices = new ServiceCollection();
        arcanicServices.AddArcanicCommandMediator();
        _serviceProvider = arcanicServices.BuildServiceProvider();
        _commandMediator = _serviceProvider.GetRequiredService<ICommandMediator>();
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
    /// Benchmarks the execution of a create user command that returns a response.
    /// Measures performance for commands with return values.
    /// </summary>
    /// <returns>The response from the create user command containing the created user information.</returns>
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

    /// <summary>
    /// Benchmarks the execution of an update user command with no return value.
    /// Measures performance for void commands that perform operations without returning data.
    /// </summary>
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

    /// <summary>
    /// Benchmarks the execution of a delete user command with additional metadata.
    /// Represents a complex command scenario with multiple properties and business logic.
    /// </summary>
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

    /// <summary>
    /// Benchmarks high-throughput command processing by executing multiple commands concurrently.
    /// Measures performance under load with parallel command execution.
    /// </summary>
    /// <param name="commandCount">The number of concurrent commands to execute.</param>
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
            tasks.Add(_commandMediator.SendAsync(command));
        }
        await Task.WhenAll(tasks);
    }
}