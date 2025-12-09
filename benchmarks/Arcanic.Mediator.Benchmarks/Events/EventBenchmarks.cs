using Arcanic.Mediator.Benchmarks.Events.UserCreated;
using Arcanic.Mediator.Benchmarks.Events.UserDeleted;
using Arcanic.Mediator.Benchmarks.Events.UserUpdated;
using Arcanic.Mediator.Event.Abstractions;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Benchmarks.Events;

[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class EventBenchmarks
{
    private IServiceProvider _serviceProvider = default!;
    private IEventPublisher _eventPublisher = default!;

    [GlobalSetup]
    public void Setup()
    {
        var arcanicServices = new ServiceCollection();
        arcanicServices.AddArcanicMediator();
        _serviceProvider = arcanicServices.BuildServiceProvider();
        _eventPublisher = _serviceProvider.GetRequiredService<IEventPublisher>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_serviceProvider is IDisposable arcanicDisposable)
            arcanicDisposable.Dispose();
    }

    [Benchmark]
    [BenchmarkCategory("SimpleEvent")]
    public async Task UserCreatedEvent()
    {
        var eventObj = new UserCreatedEvent
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _eventPublisher.PublishAsync(eventObj);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexEvent")]
    public async Task UserUpdatedEvent()
    {
        var eventObj = new UserUpdatedEvent
        {
            Id = 1,
            OldName = "Old Name",
            NewName = "New Name",
            OldEmail = "old@example.com",
            NewEmail = "new@example.com",
            UpdatedAt = DateTime.UtcNow
        };
        await _eventPublisher.PublishAsync(eventObj);
    }

    [Benchmark]
    [BenchmarkCategory("EventWithCleanup")]
    public async Task UserDeletedEvent()
    {
        var eventObj = new UserDeletedEvent
        {
            Id = 1,
            Reason = "User requested account deletion",
            DeletedAt = DateTime.UtcNow
        };
        await _eventPublisher.PublishAsync(eventObj);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task MultipleEvents(int eventCount)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= eventCount; i++)
        {
            var eventObj = new UserCreatedEvent
            {
                Id = i,
                Name = $"User {i}",
                Email = $"user{i}@example.com",
                CreatedAt = DateTime.UtcNow
            };
            tasks.Add(_eventPublisher.PublishAsync(eventObj).AsTask());
        }
        await Task.WhenAll(tasks);
    }
}