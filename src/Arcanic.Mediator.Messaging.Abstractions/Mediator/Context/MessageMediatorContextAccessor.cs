namespace Arcanic.Mediator.Messaging.Abstractions.Mediator.Context;

/// <summary>
/// Provides static access to the current message mediator context within the execution flow.
/// This class uses AsyncLocal storage to maintain context across async operations and threads,
/// enabling handlers and strategies to access the mediation context without explicit parameter passing.
/// </summary>
public class MessageMediatorContextAccessor
{
    /// <summary>
    /// The async-local storage for the current message mediator context.
    /// </summary>
    private static readonly AsyncLocal<IMessageMediatorContext?> ExecutionContextLocal = new();

    /// <summary>
    /// Gets or sets the current message mediator context for the executing flow.
    /// </summary>
    /// <value>The current message mediator context.</value>
    /// <exception cref="ArgumentNullException">Thrown when attempting to get the context when no context is set.</exception>
    public static IMessageMediatorContext Current
    {
        get => ExecutionContextLocal.Value ?? throw new ArgumentNullException();
        set => ExecutionContextLocal.Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether a current message mediator context is available.
    /// </summary>
    /// <value>True if a context is currently set; otherwise, false.</value>
    public static bool HasCurrent => ExecutionContextLocal.Value != null;

    /// <summary>
    /// Gets the current message mediator context, or null if no context is available.
    /// This method provides safe access to the context without throwing exceptions.
    /// </summary>
    /// <returns>The current message mediator context, or null if no context is set.</returns>
    public static IMessageMediatorContext? GetCurrentOrDefault()
    {
        return ExecutionContextLocal.Value;
    }

    /// <summary>
    /// Creates a new execution context scope with the specified message mediator context.
    /// The scope automatically restores the previous context when disposed.
    /// </summary>
    /// <param name="context">The message mediator context to set for the scope duration.</param>
    /// <returns>A disposable scope that manages the context lifetime.</returns>
    public static ExecutionContextScope CreateScope(IMessageMediatorContext context)
    {
        return new ExecutionContextScope(context);
    }

    /// <summary>
    /// Represents a disposable scope that manages the lifetime of a message mediator context.
    /// When disposed, the scope restores the previous context that was active before the scope was created.
    /// </summary>
    public sealed class ExecutionContextScope : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The context that was active before this scope was created.
        /// </summary>
        private readonly IMessageMediatorContext? _previousContext;
        
        /// <summary>
        /// Indicates whether this scope has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContextScope"/> class.
        /// Sets the new context and stores the previous context for restoration.
        /// </summary>
        /// <param name="newContext">The new message mediator context to set for this scope.</param>
        internal ExecutionContextScope(IMessageMediatorContext newContext)
        {
            _previousContext = GetCurrentOrDefault();
            ExecutionContextLocal.Value = newContext;
        }

        /// <summary>
        /// Asynchronously disposes the scope and restores the previous context.
        /// </summary>
        /// <returns>A completed ValueTask.</returns>
        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Disposes the scope and restores the previous message mediator context.
        /// This method is idempotent and can be called multiple times safely.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            ExecutionContextLocal.Value = _previousContext;
            _disposed = true;
        }
    }
}
