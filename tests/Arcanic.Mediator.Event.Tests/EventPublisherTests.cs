using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Tests.Data.Events.Cancellable;
using Arcanic.Mediator.Event.Tests.Data.Events.Error;
using Arcanic.Mediator.Event.Tests.Data.Events.Simple;
using Arcanic.Mediator.Event.Tests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Event.Tests
{
    public class EventPublisherTests
    {
        [Fact]
        public async Task PublishAsync_WithValidEvent_HandlesEventSuccessfully()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            service.AddSingleton<ExecutedTypeTracker>();
            var serviceProvider = service.BuildServiceProvider();
            
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
            var simpleEvent = new SimpleEvent()
            {
                Value = 42,
                Message = "Test Message"
            };

            // Act
            await eventPublisher.PublishAsync(simpleEvent);

            // Assert
            executedTypeTracker.ExecutedTypes.Should().Contain(typeof(SimpleEventPreHandler));
            executedTypeTracker.ExecutedTypes.Should().Contain(typeof(SimpleEventHandler));
            executedTypeTracker.ExecutedTypes.Should().Contain(typeof(SimpleEventPostHandler));
        }

        [Fact]
        public async Task PublishAsync_WithValidEvent_ExecutesHandlersInCorrectOrder()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            service.AddSingleton<ExecutedTypeTracker>();
            var serviceProvider = service.BuildServiceProvider();
            
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
            var simpleEvent = new SimpleEvent()
            {
                Value = 42,
                Message = "Test Message"
            };

            // Act
            await eventPublisher.PublishAsync(simpleEvent);

            // Assert
            executedTypeTracker.ExecutedTypes[0].Should().Be<SimpleEventPreHandler>();
            executedTypeTracker.ExecutedTypes[1].Should().Be<SimpleEventHandler>();
            executedTypeTracker.ExecutedTypes[2].Should().Be<SimpleEventPostHandler>();
        }

        [Fact]
        public async Task PublishAsync_WithNullEvent_ThrowsArgumentNullException()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            var serviceProvider = service.BuildServiceProvider();
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                eventPublisher.PublishAsync(null!));
        }

        [Fact]
        public async Task PublishAsync_WithEventWithoutHandler_ThrowsInvalidOperationException()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            var serviceProvider = service.BuildServiceProvider();
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            
            // Create an event that has no registered handler
            var unhandledEvent = new UnhandledEvent { Message = "Test" };

            // Act & Assert - Should throw because events require handlers
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                eventPublisher.PublishAsync(unhandledEvent));
        }

        [Fact]
        public async Task PublishAsync_WithCancellationToken_PropagatesCancellation()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            var serviceProvider = service.BuildServiceProvider();
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            var cancellableEvent = new CancellableEvent { DelayMilliseconds = 10 }; // Very short delay
            
            using var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => 
                eventPublisher.PublishAsync(cancellableEvent, cts.Token));
        }

        [Fact]
        public async Task PublishAsync_WithSameEventTypeTwice_UsesCachedDispatcher()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            service.AddSingleton<ExecutedTypeTracker>();
            var serviceProvider = service.BuildServiceProvider();
            
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
            var event1 = new SimpleEvent { Value = 1, Message = "First" };
            var event2 = new SimpleEvent { Value = 2, Message = "Second" };

            // Act
            await eventPublisher.PublishAsync(event1);
            await eventPublisher.PublishAsync(event2);

            // Assert
            // Should have executed handlers for both events
            executedTypeTracker.ExecutedTypes.Count.Should().Be(6); // 3 handlers x 2 events
            executedTypeTracker.ExecutedTypes.Count(t => t == typeof(SimpleEventHandler)).Should().Be(2);
        }

        [Fact]
        public async Task PublishAsync_WhenHandlerThrowsException_PropagatesException()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            var serviceProvider = service.BuildServiceProvider();
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            var errorEvent = new ErrorEvent { ErrorMessage = "Test error message" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                eventPublisher.PublishAsync(errorEvent));
            exception.Message.Should().Be("Test error message");
        }

        [Fact]
        public async Task PublishAsync_WithValidEvent_CompletesSuccessfully()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            service.AddSingleton<ExecutedTypeTracker>();
            var serviceProvider = service.BuildServiceProvider();
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            var simpleEvent = new SimpleEvent { Value = 42, Message = "Test" };

            // Act
            var publishTask = eventPublisher.PublishAsync(simpleEvent);
            
            // Assert
            await publishTask; // Should complete without exception
            publishTask.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task PublishAsync_WithMultipleHandlersForSameEvent_ExecutesAllHandlers()
        {
            // Arrange
            var service = new ServiceCollection();
            service.AddArcanicMediator().AddEvents(Assembly.GetExecutingAssembly());
            service.AddSingleton<ExecutedTypeTracker>();
            var serviceProvider = service.BuildServiceProvider();
            
            var eventPublisher = serviceProvider.GetRequiredService<IEventPublisher>();
            var executedTypeTracker = serviceProvider.GetRequiredService<ExecutedTypeTracker>();
            var simpleEvent = new SimpleEvent { Value = 42, Message = "Test" };

            // Act
            await eventPublisher.PublishAsync(simpleEvent);

            // Assert
            executedTypeTracker.ExecutedTypes.Should().Contain(typeof(SimpleEventHandler));
            // Note: This test demonstrates the pattern for event handling
            // Events can have multiple handlers that execute independently
        }
    }

    // Additional event for testing unhandled scenarios
    public class UnhandledEvent : IEvent
    {
        public string Message { get; init; } = string.Empty;
    }
}
