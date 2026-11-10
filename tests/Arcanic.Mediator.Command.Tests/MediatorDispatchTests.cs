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
    public async Task SendAsync_WithRequestNoModuleHandles_ThrowsWithGuidance()
    {
        var mediator = BuildMediator();

        var act = () => mediator.SendAsync(new PlainRequest());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*{typeof(PlainRequest).FullName}*AddCommands*");
    }

    [Fact]
    public async Task SendAsync_WithCommandHavingTwoResultTypes_ThrowsInvalidOperationException()
    {
        var mediator = BuildMediator();

        var act = () => mediator.SendAsync((IRequest) new AmbiguousCommand());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*ICommand<T> more than once*");
    }

    [Fact]
    public async Task SendAsync_WithCommandWithResult_InfersResponseType()
    {
        var mediator = BuildMediator();

        SimpleCommandResponse response = await mediator.SendAsync(new SimpleCommand { Value = 3 });

        response.Result.Should().Be(3);
    }

    [Fact]
    public async Task SendAsync_WithVoidCommand_RunsHandler()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator().AddCommands(Assembly.GetExecutingAssembly());
        services.AddSingleton<ExecutedTypeTracker>();
        var provider = services.BuildServiceProvider();

        await provider.GetRequiredService<IMediator>().SendAsync(new VoidCommand());

        provider.GetRequiredService<ExecutedTypeTracker>().ExecutedTypes.Should().Contain(typeof(VoidCommandHandler));
    }

    [Fact]
    public async Task SendAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        var mediator = BuildMediator();

        var act = () => mediator.SendAsync((IRequest) null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SendAsync_OnFakeMediator_ReturnsFakeResponse()
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
        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
            Task.FromResult((TResponse) response!);

        public Task SendAsync(IRequest request, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
