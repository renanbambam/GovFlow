using MediatR;

namespace GovFlow.Application.Process.Commands.CreateProcessType;

/// <summary>Defines a process template together with its ordered workflow steps.</summary>
public sealed record CreateProcessTypeCommand(
    string Name,
    string Description,
    Guid OrganizationId,
    IReadOnlyList<WorkflowStepInput> Steps) : IRequest<Guid>;

/// <summary>A single workflow step to add to the new process type, in declaration order.</summary>
public sealed record WorkflowStepInput(
    string Name,
    string Description,
    Guid? AssignableDepartmentId = null,
    int? SlaHours = null,
    IReadOnlyList<string>? RequiredDocuments = null);
