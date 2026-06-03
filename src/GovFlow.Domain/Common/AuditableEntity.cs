namespace GovFlow.Domain.Common;

/// <summary>
/// An entity that tracks when it was created and last modified. All timestamps are UTC.
/// Derived types call <see cref="Touch"/> whenever they mutate state.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; protected set; }

    public DateTime? UpdatedAt { get; protected set; }

    protected AuditableEntity() => CreatedAt = DateTime.UtcNow;

    protected void Touch() => UpdatedAt = DateTime.UtcNow;
}
