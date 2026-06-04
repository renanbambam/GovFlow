namespace GovFlow.Application.Process.Dtos;

public sealed record ProcessSummaryDto(
    Guid Id,
    Guid ProcessTypeId,
    Guid OrganizationId,
    string Title,
    string Status,
    string Priority,
    DateTime OpenedAt,
    DateTime? ClosedAt);

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
