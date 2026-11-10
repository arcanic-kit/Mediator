using Arcanic.Mediator.Abstractions.Configuration;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Abstractions;
using Arcanic.Mediator.Event.Abstractions.Pipeline;
using Arcanic.Mediator.Event.Tests.Data.Events.MultiHandler;
using Arcanic.Mediator.Tests.Shared;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Event.Tests;

public class RegistrationTests
{
    [Theory]
    [InlineData(typeof(NotABehavior))]
    [InlineData(typeof(AbstractEventBehavior<,>))]
    public void AddEventPipelineBehavior_WithInvalidType_ThrowsArgumentException(Type behaviorType)
    {
        var builder = new ServiceCollection().AddArcanicMediator();

        var act = () => builder.AddEventPipelineBehavior(behaviorType);

        act.Should().Throw<ArgumentException>().WithParameterName("eventPipelineBehaviorType");
    }

    [Fact]
    public void AddEvents_CalledTwice_RegistersSinglePublisher()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator()
            .AddEvents(Assembly.GetExecutingAssembly())
            .AddEvents(Assembly.GetExecutingAssembly());

        services.Count(d => d.ServiceType == typeof(IPublisher)).Should().Be(1);
    }

    [Theory]
    [InlineData(InstanceLifetime.Transient)]
    [InlineData(InstanceLifetime.Scoped)]
    [InlineData(InstanceLifetime.Singleton)]
    public async Task PublishAsync_WithEachLifetime_RunsAllHandlersInsideScope(InstanceLifetime lifetime)
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator(config => config.InstanceLifetime = lifetime)
            .AddEvents(Assembly.GetExecutingAssembly());
        services.AddSingleton<ExecutedTypeTracker>();
        var root = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        using var scope = root.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IPublisher>().PublishAsync(new MultiHandlerEvent());

        root.GetRequiredService<ExecutedTypeTracker>().ExecutedTypes.Should().HaveCount(2);
    }

    private class NotABehavior;

    private abstract class AbstractEventBehavior<TEvent, TResponse> : IEventPipelineBehavior<TEvent, TResponse>
        where TEvent : IEvent
    {
        public abstract Task<TResponse> HandleAsync(TEvent message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default);
    }
}
