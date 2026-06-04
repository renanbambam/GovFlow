using GovFlow.Application.Process.Commands.CreateProcessType;

namespace GovFlow.API.Contracts.Processes;

public sealed record CreateProcessTypeRequest(
    string Name,
    string Description,
    Guid OrganizationId,
    IReadOnlyList<WorkflowStepRequest> Steps)
{
    public CreateProcessTypeCommand ToCommand()
        => new(Name, Description, OrganizationId, Steps.Select(step => step.ToInput()).ToList());
}

public sealed record WorkflowStepRequest(
    string Name,
    string Description,
    Guid? AssignableDepartmentId = null,
    int? SlaHours = null,
    IReadOnlyList<string>? RequiredDocuments = null)
{
    public WorkflowStepInput ToInput() => new(Name, Description, AssignableDepartmentId, SlaHours, RequiredDocuments);
}
