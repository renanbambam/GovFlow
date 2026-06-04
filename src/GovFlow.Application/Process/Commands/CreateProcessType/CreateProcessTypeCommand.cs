using MediatR;

namespace GovFlow.Application.Process.Commands.CreateProcessType;

public sealed record CreateProcessTypeCommand(
    string Name,
    string Description,
    Guid OrganizationId,
    IReadOnlyList<WorkflowStepInput> Steps) : IRequest<Guid>;

public sealed record WorkflowStepInput(
    string Name,
    string Description,
    Guid? AssignableDepartmentId = null,
    int? SlaHours = null,
    IReadOnlyList<string>? RequiredDocuments = null);
