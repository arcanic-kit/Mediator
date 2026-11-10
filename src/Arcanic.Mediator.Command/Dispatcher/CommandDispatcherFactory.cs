using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Request.Dispatcher;

namespace Arcanic.Mediator.Command.Dispatcher;

/// <summary>
/// Creates <see cref="CommandDispatcher{TCommand}"/> or <see cref="CommandDispatcher{TCommand, TResponse}"/>
/// instances for command types.
/// </summary>
internal sealed class CommandDispatcherFactory : IRequestDispatcherFactory
{
    /// <inheritdoc />
    public RequestDispatcherBase? CreateDispatcher(Type requestType)
    {
        if (!typeof(ICommand).IsAssignableFrom(requestType))
        {
            return null;
        }

        var resultInterfaces = requestType.GetInterfaces()
            .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICommand<>))
            .ToArray();

        var dispatcherType = resultInterfaces.Length switch
        {
            0 => typeof(CommandDispatcher<>).MakeGenericType(requestType),
            1 => typeof(CommandDispatcher<,>).MakeGenericType(requestType, resultInterfaces[0].GetGenericArguments()[0]),
            _ => throw new InvalidOperationException(
                $"Command type '{requestType.FullName}' implements ICommand<T> more than once; a command must have a single result type.")
        };

        return (RequestDispatcherBase)Activator.CreateInstance(dispatcherType)!;
    }
}
