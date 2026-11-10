namespace Arcanic.Mediator.Abstractions.Configuration;

/// <summary>
/// Represents the configuration settings for the Arcanic Mediator service.
/// Contains options and settings that control the behavior of the mediator.
/// </summary>
public class ArcanicMediatorConfiguration
{
    /// <summary>
    /// Gets or sets the service lifetime of the mediator entry points (<c>IMediator</c> and <c>IPublisher</c>).
    /// Defaults to <see cref="InstanceLifetime.Transient"/>.
    /// </summary>
    /// <remarks>
    /// The mediator resolves handlers from the service provider it was itself resolved from. A
    /// <see cref="InstanceLifetime.Singleton"/> mediator therefore resolves handlers from the root provider,
    /// which is only valid when <see cref="HandlerLifetime"/> is also <see cref="InstanceLifetime.Singleton"/>.
    /// </remarks>
    public InstanceLifetime MediatorLifetime { get; set; } = InstanceLifetime.Transient;

    /// <summary>
    /// Gets or sets the service lifetime of handlers, pre/post handlers and pipeline behaviors.
    /// Defaults to <see cref="InstanceLifetime.Transient"/>.
    /// </summary>
    /// <remarks>
    /// Use <see cref="InstanceLifetime.Scoped"/> or <see cref="InstanceLifetime.Transient"/> when handlers depend on
    /// scoped services such as a database context; a <see cref="InstanceLifetime.Singleton"/> handler would capture them.
    /// </remarks>
    public InstanceLifetime HandlerLifetime { get; set; } = InstanceLifetime.Transient;

    /// <summary>
    /// Gets or sets a single lifetime for both the mediator and its handlers.
    /// Setting this property assigns <see cref="MediatorLifetime"/> and <see cref="HandlerLifetime"/>;
    /// reading it returns <see cref="HandlerLifetime"/>.
    /// </summary>
    [Obsolete("Use MediatorLifetime and HandlerLifetime instead. This property sets both and will be removed in a future major version.")]
    public InstanceLifetime InstanceLifetime
    {
        get => HandlerLifetime;
        set
        {
            MediatorLifetime = value;
            HandlerLifetime = value;
        }
    }

    /// <summary>
    /// Ensures the configured lifetimes can work together.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="MediatorLifetime"/> is <see cref="InstanceLifetime.Singleton"/> and
    /// <see cref="HandlerLifetime"/> is <see cref="InstanceLifetime.Scoped"/>: a singleton mediator would resolve
    /// scoped handlers from the root provider.
    /// </exception>
    public void Validate()
    {
        if (MediatorLifetime == InstanceLifetime.Singleton && HandlerLifetime == InstanceLifetime.Scoped)
        {
            throw new InvalidOperationException(
                $"{nameof(MediatorLifetime)} cannot be {InstanceLifetime.Singleton} when {nameof(HandlerLifetime)} is " +
                $"{InstanceLifetime.Scoped}: a singleton mediator resolves handlers from the root service provider. " +
                $"Use a {InstanceLifetime.Scoped} or {InstanceLifetime.Transient} mediator.");
        }
    }
}
