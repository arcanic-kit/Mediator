using Arcanic.Mediator.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Query.Abstractions.Pipeline;
using Arcanic.Mediator.Query.Tests.Data.Pipelines;
using Arcanic.Mediator.Query.Tests.Data.Queries.Simple;
using Arcanic.Mediator.Request.Abstractions;
using Arcanic.Mediator.Tests.Shared;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcanic.Mediator.Query.Tests;

public class RegistrationTests
{
    [Theory]
    [InlineData(typeof(NotABehavior))]
    [InlineData(typeof(AbstractQueryBehavior<,>))]
    public void AddQueryPipelineBehavior_WithInvalidType_ThrowsArgumentException(Type behaviorType)
    {
        var builder = new ServiceCollection().AddArcanicMediator();

        var act = () => builder.AddQueryPipelineBehavior(behaviorType);

        act.Should().Throw<ArgumentException>().WithParameterName("queryPipelineBehaviorType");
    }

    [Fact]
    public async Task AddQueryPipelineBehavior_CalledTwiceWithSameType_RunsBehaviorOnce()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator()
            .AddQueryPipelineBehavior(typeof(ExampleQueryPipelineBehavior<,>))
            .AddQueryPipelineBehavior(typeof(ExampleQueryPipelineBehavior<,>))
            .AddQueries(Assembly.GetExecutingAssembly());
        services.AddSingleton<ExecutedTypeTracker>();
        var serviceProvider = services.BuildServiceProvider();

        await serviceProvider.GetRequiredService<IMediator>().SendAsync(new SimpleQuery { Value = 1 });

        serviceProvider.GetRequiredService<ExecutedTypeTracker>().ExecutedTypes
            .Count(t => t == typeof(ExampleQueryPipelineBehavior<SimpleQuery, SimpleQueryResponse>))
            .Should().Be(1);
    }

    [Fact]
    public void AddQueries_CalledTwice_RegistersSingleMediator()
    {
        var services = new ServiceCollection();
        services.AddArcanicMediator()
            .AddQueries(Assembly.GetExecutingAssembly())
            .AddQueries(Assembly.GetExecutingAssembly());

        services.Count(d => d.ServiceType == typeof(IMediator)).Should().Be(1);
    }

    private class NotABehavior;

    private abstract class AbstractQueryBehavior<TQuery, TResponse> : IQueryPipelineBehavior<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public abstract Task<TResponse> HandleAsync(TQuery message, PipelineDelegate<TResponse> next, CancellationToken cancellationToken = default);
    }
}
