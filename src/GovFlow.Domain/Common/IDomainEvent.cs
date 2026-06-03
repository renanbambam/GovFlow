namespace GovFlow.Domain.Common;

/// <summary>
/// Marker for something that happened in the domain. Kept free of any infrastructure
/// dependency (no MediatR here) so the Domain layer imports nothing external; the
/// Application layer adapts these to its own dispatch mechanism.
/// </summary>
public interface IDomainEvent
{
}
