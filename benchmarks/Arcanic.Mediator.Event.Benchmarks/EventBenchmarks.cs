using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Benchmarks.UserCreated;
using Arcanic.Mediator.Event.Benchmarks.UserDeleted;
using Arcanic.Mediator.Event.Benchmarks.UserUpdated;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Event.Benchmarks;

/// <summary>
/// Provides performance benchmarks for event publishing through the Arcanic Mediator.
/// Measures execution time and memory allocation for various event publishing scenarios.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class EventBenchmarks
{
    private IServiceProvider _serviceProvider = default!;
    private IEventPublisher _eventPublisher = default!;

    /// <summary>
    /// Initializes the dependency injection container and event publisher for benchmarking.
    /// Configures Arcanic Mediator services and builds the service provider.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        var arcanicServices = new ServiceCollection();
        arcanicServices.AddArcanicEventMediator();
        _serviceProvider = arcanicServices.BuildServiceProvider();
        _eventPublisher = _serviceProvider.GetRequiredService<IEventPublisher>();
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
    /// Benchmarks the publishing of a simple user created event.
    /// Measures performance for basic event publishing with minimal event data.
    /// </summary>
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

    /// <summary>
    /// Benchmarks the publishing of a complex user updated event containing change tracking data.
    /// Measures performance for events with more complex data structures and comparison fields.
    /// </summary>
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

    /// <summary>
    /// Benchmarks the publishing of a user deleted event that may trigger cleanup operations.
    /// Measures performance for events that typically involve resource cleanup and cascading operations.
    /// </summary>
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

    /// <summary>
    /// Benchmarks high-throughput event publishing by publishing multiple events concurrently.
    /// Measures performance under load with parallel event publishing scenarios.
    /// </summary>
    /// <param name="eventCount">The number of concurrent events to publish.</param>
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
            tasks.Add(_eventPublisher.PublishAsync(eventObj));
        }
        await Task.WhenAll(tasks);
    }
}