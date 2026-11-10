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

    /// <summary>Every mediator/handler lifetime combination except the rejected Singleton mediator + Scoped handlers.</summary>
    public static TheoryData<InstanceLifetime, InstanceLifetime> ValidLifetimeCombinations()
    {
        var data = new TheoryData<InstanceLifetime, InstanceLifetime>();
        foreach (var mediator in Enum.GetValues<InstanceLifetime>())
        foreach (var handler in Enum.GetValues<InstanceLifetime>())
        {
            if (mediator == InstanceLifetime.Singleton && handler == InstanceLifetime.Scoped)
                continue;
            data.Add(mediator, handler);
        }
        return data;
    }

    [Fact]
    public void AddArcanicMediator_ByDefault_RegistersEverythingTransient()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());

        services.Where(d => d.ServiceType.Namespace?.StartsWith("Arcanic.Mediator") == true)
            .Should().OnlyContain(d => d.Lifetime == ServiceLifetime.Transient);
    }

    [Theory]
    [MemberData(nameof(ValidLifetimeCombinations))]
    public void AddCommands_RegistersMediatorAndHandlersWithTheirOwnLifetimes(InstanceLifetime mediatorLifetime, InstanceLifetime handlerLifetime)
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator(config =>
            {
                config.MediatorLifetime = mediatorLifetime;
                config.HandlerLifetime = handlerLifetime;
            })
            .AddCommands(Assembly.GetExecutingAssembly());

        services.Single(d => d.ServiceType == typeof(IMediator)).Lifetime.Should().Be(ToServiceLifetime(mediatorLifetime));
        services.Where(d => d.ServiceType != typeof(IMediator) && d.ServiceType.Namespace?.StartsWith("Arcanic.Mediator") == true)
            .Should().NotBeEmpty()
            .And.OnlyContain(d => d.Lifetime == ToServiceLifetime(handlerLifetime));
    }

    [Fact]
    public void AddArcanicMediator_WithSingletonMediatorAndScopedHandlers_Throws()
    {
        var act = () => new ServiceCollection().AddArcanicMediator(config =>
        {
            config.MediatorLifetime = InstanceLifetime.Singleton;
            config.HandlerLifetime = InstanceLifetime.Scoped;
        });

        act.Should().Throw<InvalidOperationException>().WithMessage("*MediatorLifetime*HandlerLifetime*");
    }

    [Theory]
    [InlineData(InstanceLifetime.Transient)]
    [InlineData(InstanceLifetime.Scoped)]
    [InlineData(InstanceLifetime.Singleton)]
    public void InstanceLifetime_ObsoleteSetter_SetsMediatorAndHandlerLifetimes(InstanceLifetime lifetime)
    {
        var configuration = new ArcanicMediatorConfiguration();

#pragma warning disable CS0618 // Verifying the obsolete compatibility shim
        configuration.InstanceLifetime = lifetime;
#pragma warning restore CS0618

        configuration.MediatorLifetime.Should().Be(lifetime);
        configuration.HandlerLifetime.Should().Be(lifetime);
    }

    [Theory]
    [MemberData(nameof(ValidLifetimeCombinations))]
    public async Task SendAsync_WithEachLifetimeCombination_ResolvesAndRunsHandlerInsideScope(InstanceLifetime mediatorLifetime, InstanceLifetime handlerLifetime)
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator(config =>
            {
                config.MediatorLifetime = mediatorLifetime;
                config.HandlerLifetime = handlerLifetime;
            })
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

    private static ServiceLifetime ToServiceLifetime(InstanceLifetime lifetime) => lifetime switch
    {
        InstanceLifetime.Singleton => ServiceLifetime.Singleton,
        InstanceLifetime.Scoped => ServiceLifetime.Scoped,
        _ => ServiceLifetime.Transient
    };

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
