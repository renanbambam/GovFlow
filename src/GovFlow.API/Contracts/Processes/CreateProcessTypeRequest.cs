using GovFlow.Application.Process.Commands.CreateProcessType;

namespace GovFlow.API.Contracts.Processes;

/// <summary>Payload to define a process template and its ordered workflow steps.</summary>
/// <param name="Name">Process type name. Example: "Vacation Request".</param>
/// <param name="Description">What this process is for.</param>
/// <param name="OrganizationId">Owning organization.</param>
/// <param name="Steps">Ordered list of workflow steps (at least one required).</param>
public sealed record CreateProcessTypeRequest(
    string Name,
    string Description,
    Guid OrganizationId,
    IReadOnlyList<WorkflowStepRequest> Steps)
{
    public CreateProcessTypeCommand ToCommand()
        => new(Name, Description, OrganizationId, Steps.Select(step => step.ToInput()).ToList());
}

/// <summary>A single workflow step in declaration order.</summary>
/// <param name="Name">Step name. Example: "Manager approval".</param>
/// <param name="Description">What happens in this step.</param>
/// <param name="AssignableDepartmentId">Default department to handle the step, if any.</param>
/// <param name="SlaHours">Resolution target in hours, if any.</param>
/// <param name="RequiredDocuments">Document names required to complete the step.</param>
public sealed record WorkflowStepRequest(
    string Name,
    string Description,
    Guid? AssignableDepartmentId = null,
    int? SlaHours = null,
    IReadOnlyList<string>? RequiredDocuments = null)
{
    public WorkflowStepInput ToInput() => new(Name, Description, AssignableDepartmentId, SlaHours, RequiredDocuments);
}
