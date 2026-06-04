namespace GovFlow.Application.Process.Dtos;

public sealed record ProcessTypeDto(
    Guid Id,
    string Name,
    string Description,
    Guid OrganizationId,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<WorkflowStepDto> Steps);

public sealed record WorkflowStepDto(
    Guid Id,
    string Name,
    string Description,
    int Order,
    Guid? AssignableDepartmentId,
    int? SlaHours,
    IReadOnlyList<string> RequiredDocuments);
