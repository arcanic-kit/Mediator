using Arcanic.Mediator.Abstractions;
using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Command.Tests.Data.Commands.Simple;
using Arcanic.Mediator.Command.Tests.Data.Commands.VoidCommand;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Tests.Shared;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Command.Tests;

public class MediatorDispatchTests
{
    [Fact]
    public async Task DispatchAsync_WithRequestNoModuleHandles_ThrowsWithGuidance()
    {
        var mediator = BuildMediator();

        var act = () => mediator.DispatchAsync(new PlainRequest());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{typeof(PlainRequest).FullName}*AddCommands*");
    }

    [Fact]
    public async Task DispatchAsync_WithCommandHavingTwoResultTypes_ThrowsInvalidOperationException()
    {
        var mediator = BuildMediator();

        var act = () => mediator.DispatchAsync(new AmbiguousCommand());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*ICommand<T> more than once*");
    }

    [Fact]
    public async Task DispatchAsync_WithCommand_ReturnsTypedResponseAsObject()
    {
        var mediator = BuildMediator();

        var result = await mediator.DispatchAsync(new SimpleCommand { Value = 3 });

        result.Should().BeOfType<SimpleCommandResponse>().Which.Result.Should().Be(3);
    }

    [Fact]
    public async Task DispatchAsync_WithVoidCommand_ReturnsUnit()
    {
        var mediator = BuildMediator();

        var result = await mediator.DispatchAsync(new VoidCommand());

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task SendAsync_OnFakeMediator_UsesDispatchAsync()
    {
        IMediator mediator = new FakeMediator(new SimpleCommandResponse { Result = 99, Message = "fake" });

        var response = await mediator.SendAsync(new SimpleCommand { Value = 1 });

        response.Result.Should().Be(99);
    }

    private static IMediator BuildMediator()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        services.AddSingleton<ExecutedTypeTracker>();
        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    private class PlainRequest : IRequest;

    private class AmbiguousCommand : ICommand<int>, ICommand<string>;

    private class FakeMediator(object? response) : IMediator
    {
        public Task<object?> DispatchAsync(IRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(response);
    }
}
