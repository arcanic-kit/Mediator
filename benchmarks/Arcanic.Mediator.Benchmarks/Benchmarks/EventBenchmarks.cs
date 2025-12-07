using Arcanic.Mediator.Benchmarks.Arcanic.Events;
using Arcanic.Mediator.Benchmarks.Configuration;
using Arcanic.Mediator.Benchmarks.MediatR.Events;
using Arcanic.Mediator.Event.Abstractions;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Arcanic.Mediator.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks comparing event performance between MediatR and Arcanic Mediator
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public class EventBenchmarks
{
    private IServiceProvider _mediatRServiceProvider = default!;
    private IServiceProvider _arcanicServiceProvider = default!;

    private IMediator _mediatr = default!;
    private IEventPublisher _arcanicEventPublisher = default!;

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
        _arcanicEventPublisher = _arcanicServiceProvider.GetRequiredService<IEventPublisher>();
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
    [BenchmarkCategory("SimpleEvent")]
    public async Task MediatR_UserCreatedEvent()
    {
        var notification = new UserCreatedNotification(1, "John Doe", "john@example.com", DateTime.UtcNow);
        await _mediatr.Publish(notification);
    }

    [Benchmark]
    [BenchmarkCategory("SimpleEvent")]
    public async Task Arcanic_UserCreatedEvent()
    {
        var eventObj = new UserCreatedArcanicEvent
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _arcanicEventPublisher.PublishAsync(eventObj);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexEvent")]
    public async Task MediatR_UserUpdatedEvent()
    {
        var notification = new UserUpdatedNotification(
            1, "Old Name", "New Name", "old@example.com", "new@example.com", DateTime.UtcNow);
        await _mediatr.Publish(notification);
    }

    [Benchmark]
    [BenchmarkCategory("ComplexEvent")]
    public async Task Arcanic_UserUpdatedEvent()
    {
        var eventObj = new UserUpdatedArcanicEvent
        {
            Id = 1,
            OldName = "Old Name",
            NewName = "New Name",
            OldEmail = "old@example.com",
            NewEmail = "new@example.com",
            UpdatedAt = DateTime.UtcNow
        };
        await _arcanicEventPublisher.PublishAsync(eventObj);
    }

    [Benchmark]
    [BenchmarkCategory("EventWithCleanup")]
    public async Task MediatR_UserDeletedEvent()
    {
        var notification = new UserDeletedNotification(1, "User requested account deletion", DateTime.UtcNow);
        await _mediatr.Publish(notification);
    }

    [Benchmark]
    [BenchmarkCategory("EventWithCleanup")]
    public async Task Arcanic_UserDeletedEvent()
    {
        var eventObj = new UserDeletedArcanicEvent
        {
            Id = 1,
            Reason = "User requested account deletion",
            DeletedAt = DateTime.UtcNow
        };
        await _arcanicEventPublisher.PublishAsync(eventObj);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task MediatR_MultipleEvents(int eventCount)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= eventCount; i++)
        {
            var notification = new UserCreatedNotification(i, $"User {i}", $"user{i}@example.com", DateTime.UtcNow);
            tasks.Add(_mediatr.Publish(notification));
        }
        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("HighThroughput")]
    [Arguments(100)]
    public async Task Arcanic_MultipleEvents(int eventCount)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= eventCount; i++)
        {
            var eventObj = new UserCreatedArcanicEvent
            {
                Id = i,
                Name = $"User {i}",
                Email = $"user{i}@example.com",
                CreatedAt = DateTime.UtcNow
            };
            tasks.Add(_arcanicEventPublisher.PublishAsync(eventObj).AsTask());
        }
        await Task.WhenAll(tasks);
    }
}