namespace GovFlow.Application.Process.Dtos;

/// <summary>Lightweight read model used in process listings.</summary>
public sealed record ProcessSummaryDto(
    Guid Id,
    Guid ProcessTypeId,
    Guid OrganizationId,
    string Title,
    string Status,
    string Priority,
    DateTime OpenedAt,
    DateTime? ClosedAt);

/// <summary>Full read model for a single process instance, including its steps.</summary>
public sealed record ProcessInstanceDto(
    Guid Id,
    Guid ProcessTypeId,
    Guid OrganizationId,
    string Title,
    string Description,
    Guid RequesterId,
    Guid? CurrentStepId,
    string Status,
    string Priority,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    DateTime? DueAt,
    IReadOnlyList<ProcessInstanceStepDto> Steps);

/// <summary>Read model for a single step of a running process.</summary>
public sealed record ProcessInstanceStepDto(
    Guid Id,
    Guid WorkflowStepId,
    int Sequence,
    string Status,
    Guid? AssignedUserId,
    Guid? AssignedDepartmentId,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? Notes);
