using GovFlow.Application.Process.Commands.OpenProcessInstance;
using GovFlow.Domain.Process.Enums;

namespace GovFlow.API.Contracts.Processes;

/// <summary>Payload to open a running process instance from a process type.</summary>
/// <param name="ProcessTypeId">Template the instance is created from.</param>
/// <param name="RequesterId">User opening the process.</param>
/// <param name="Title">Short title. Example: "Vacation - July".</param>
/// <param name="Description">Details of the request.</param>
/// <param name="Priority">Low, Normal, High or Critical. Defaults to Normal.</param>
public sealed record OpenProcessInstanceRequest(
    Guid ProcessTypeId,
    Guid RequesterId,
    string Title,
    string Description,
    ProcessPriority Priority = ProcessPriority.Normal)
{
    public OpenProcessInstanceCommand ToCommand() => new(ProcessTypeId, RequesterId, Title, Description, Priority);
}
