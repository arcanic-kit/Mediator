using Arcanic.Mediator.Query.Abstractions;

namespace Arcanic.Mediator.Query.Tests.Data.Queries.Error;

public class ErrorQuery : IQuery<ErrorQueryResponse>
{
    public string ErrorMessage { get; set; } = string.Empty;
}