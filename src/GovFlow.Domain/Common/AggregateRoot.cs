namespace GovFlow.Domain.Common;

/// <summary>
/// Consistency boundary of the domain. An aggregate root is the only entry point for
/// mutating the objects it owns and is responsible for recording the domain events that
/// result from those changes. Events are buffered here and drained by the Application
/// layer after the aggregate is persisted.
/// </summary>
public abstract class AggregateRoot : AuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
