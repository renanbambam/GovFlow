using GovFlow.Domain.Common;

namespace GovFlow.Domain.Process.Events;

/// <summary>Raised when a new process instance is opened from a process type.</summary>
public sealed record ProcessInstanceOpenedEvent(
    Guid ProcessInstanceId,
    Guid ProcessTypeId,
    Guid RequesterId) : IDomainEvent;

/// <summary>Raised when the current step of a process is completed.</summary>
public sealed record ProcessStepCompletedEvent(
    Guid ProcessInstanceId,
    Guid StepId) : IDomainEvent;

/// <summary>Raised when a step is sent back to the previous step.</summary>
public sealed record ProcessStepReturnedEvent(
    Guid ProcessInstanceId,
    Guid StepId) : IDomainEvent;

/// <summary>Raised when a process reaches its final step and is resolved.</summary>
public sealed record ProcessInstanceResolvedEvent(
    Guid ProcessInstanceId) : IDomainEvent;

/// <summary>Raised when a process is cancelled before resolution.</summary>
public sealed record ProcessInstanceCancelledEvent(
    Guid ProcessInstanceId) : IDomainEvent;
