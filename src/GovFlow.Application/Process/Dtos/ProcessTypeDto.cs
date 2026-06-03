namespace GovFlow.Application.Process.Dtos;

/// <summary>Read model for a process type and its workflow steps.</summary>
public sealed record ProcessTypeDto(
    Guid Id,
    string Name,
    string Description,
    Guid OrganizationId,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<WorkflowStepDto> Steps);

/// <summary>Read model for a single workflow step.</summary>
public sealed record WorkflowStepDto(
    Guid Id,
    string Name,
    string Description,
    int Order,
    Guid? AssignableDepartmentId,
    int? SlaHours,
    IReadOnlyList<string> RequiredDocuments);
