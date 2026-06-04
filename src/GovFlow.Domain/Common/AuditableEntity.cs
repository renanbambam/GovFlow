namespace GovFlow.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; protected set; }

    public DateTime? UpdatedAt { get; protected set; }

    protected AuditableEntity() => CreatedAt = DateTime.UtcNow;

    protected void Touch() => UpdatedAt = DateTime.UtcNow;
}
