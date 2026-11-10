using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Abstractions.Configuration;
using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Abstractions.Pipeline;
using Arcanic.Mediator.Command.Tests.Data.Commands.Simple;
using Arcanic.Mediator.Command.Tests.Data.Pipelines;
using Arcanic.Mediator.Request;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Request.Abstractions.Pipeline;
using Arcanic.Mediator.Tests.Shared;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Command.Tests;

public class RegistrationTests
{
    [Theory]
    [InlineData(typeof(NotABehavior))]
    [InlineData(typeof(AbstractGenericBehavior<,>))]
    public void AddPipelineBehavior_WithInvalidType_ThrowsArgumentException(Type behaviorType)
    {
        var builder = new ServiceCollection().AddArcanicMediator();

        var act = () => builder.AddPipelineBehavior(behaviorType);

        act.Should().Throw<ArgumentException>().WithParameterName("pipelineBehaviorType");
    }

    [Theory]
    [InlineData(typeof(NotABehavior))]
    [InlineData(typeof(AbstractRequestBehavior<,>))]
    public void AddRequestPipelineBehavior_WithInvalidType_ThrowsArgumentException(Type behaviorType)
    {
        var builder = new ServiceCollection().AddArcanicMediator();

        var act = () => builder.AddRequestPipelineBehavior(behaviorType);

        act.Should().Throw<ArgumentException>().WithParameterName("requestPipelineBehaviorType");
    }

    [Theory]
    [InlineData(typeof(NotABehavior))]
    [InlineData(typeof(AbstractCommandBehavior<,>))]
    public void AddCommandPipelineBehavior_WithInvalidType_ThrowsArgumentException(Type behaviorType)
    {
        var builder = new ServiceCollection().AddArcanicMediator();

        var act = () => builder.AddCommandPipelineBehavior(behaviorType);

        act.Should().Throw<ArgumentException>().WithParameterName("commandPipelineBehaviorType");
    }

    [Fact]
    public void AddPipelineBehavior_WithNull_ThrowsArgumentNullException()
    {
        var builder = new ServiceCollection().AddArcanicMediator();

        var act = () => builder.AddPipelineBehavior(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCommandPipelineBehavior_CalledTwiceWithSameType_RunsBehaviorOnce()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator()
            .AddCommandPipelineBehavior(typeof(ExampleCommandPipelineBehavior<,>))
            .AddCommandPipelineBehavior(typeof(ExampleCommandPipelineBehavior<,>))
            .AddCommands(Assembly.GetExecutingAssembly());
        services.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = services.BuildServiceProvider();

        await serviceProvider.GetRequiredService<IMediator>().SendAsync(new SimpleCommand { Value = 1 });

        serviceProvider.GetRequiredService<ExecutedTypeTracker>().ExecutedTypes
            .Count(t => t == typeof(ExampleCommandPipelineBehavior<SimpleCommand, SimpleCommandResponse>))
            .Should().Be(1);
    }

    [Theory]
    [InlineData(InstanceLifetime.Transient, ServiceLifetime.Transient)]
    [InlineData(InstanceLifetime.Scoped, ServiceLifetime.Scoped)]
    [InlineData(InstanceLifetime.Singleton, ServiceLifetime.Singleton)]
    public void AddCommands_RegistersServicesWithConfiguredLifetime(InstanceLifetime configured, ServiceLifetime expected)
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator(config => config.InstanceLifetime = configured)
            .AddCommands(Assembly.GetExecutingAssembly());

        services.Single(d => d.ServiceType == typeof(IMediator)).Lifetime.Should().Be(expected);
        services.Where(d => d.ServiceType.Namespace?.StartsWith("Arcanic.Mediator") == true)
            .Should().OnlyContain(d => d.Lifetime == expected);
    }

    [Theory]
    [InlineData(InstanceLifetime.Transient)]
    [InlineData(InstanceLifetime.Scoped)]
    [InlineData(InstanceLifetime.Singleton)]
    public async Task SendAsync_WithEachLifetime_ResolvesAndRunsHandlerInsideScope(InstanceLifetime lifetime)
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator(config => config.InstanceLifetime = lifetime)
            .AddCommands(Assembly.GetExecutingAssembly());
        services.AddSingleton<ExecutedTypeTracker>();
        var root = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        using var scope = root.CreateScope();
        var response = await scope.ServiceProvider.GetRequiredService<IMediator>().SendAsync(new SimpleCommand { Value = 7 });

        response.Result.Should().Be(7);
    }

    [Fact]
    public void AddArcanicMediator_WithCustomMediatorAlreadyRegistered_KeepsCustomMediator()
    {
        var services = new ServiceCollection();
        services.AddTransient<IMediator, CustomMediator>();

        services.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());

        services.Where(d => d.ServiceType == typeof(IMediator))
            .Should().ContainSingle()
            .Which.ImplementationType.Should().Be<CustomMediator>();
    }

    [Fact]
    public void AddCommands_WithOpenGenericHandlerClassInAssembly_SkipsIt()
    {
        var services = new ServiceCollection();

        services.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        services.AddSingleton<ExecutedTypeTracker>();

        services.Should().NotContain(d => d.ImplementationType == typeof(OpenGenericPreHandler<>));
        services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true }).Should().NotBeNull();
    }

    private class NotABehavior;

    // Discovered by assembly scanning; must be ignored rather than registered as an unusable service.
    private class OpenGenericPreHandler<TCommand> : Abstractions.Handler.ICommandPreHandler<TCommand>
        where TCommand : ICommand
    {
        public Task HandleAsync(TCommand message, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private abstract class AbstractGenericBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IMessage
    {
        public abstract Task<TResponse> HandleAsync(TMessage message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default);
    }

    private abstract class AbstractRequestBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest
    {
        public abstract Task<TResponse> HandleAsync(TRequest message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default);
    }

    private abstract class AbstractCommandBehavior<TCommand, TResponse> : ICommandPipelineBehavior<TCommand, TResponse>
        where TCommand : ICommand
    {
        public abstract Task<TResponse> HandleAsync(TCommand message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default);
    }

    private class CustomMediator(IServiceProvider serviceProvider) : Arcanic.Mediator.Request.Mediator(serviceProvider);
}
