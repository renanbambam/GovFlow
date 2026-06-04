using GovFlow.Domain.Common;

namespace GovFlow.Domain.Process.Events;

public sealed record ProcessInstanceOpenedEvent(
    Guid ProcessInstanceId,
    Guid ProcessTypeId,
    Guid RequesterId) : IDomainEvent;

public sealed record ProcessStepCompletedEvent(
    Guid ProcessInstanceId,
    Guid StepId) : IDomainEvent;

public sealed record ProcessStepReturnedEvent(
    Guid ProcessInstanceId,
    Guid StepId) : IDomainEvent;

public sealed record ProcessInstanceResolvedEvent(
    Guid ProcessInstanceId) : IDomainEvent;

public sealed record ProcessInstanceCancelledEvent(
    Guid ProcessInstanceId) : IDomainEvent;
