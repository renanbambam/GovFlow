using GovFlow.Application.Process.Commands.OpenProcessInstance;
using GovFlow.Domain.Process.Enums;

namespace GovFlow.API.Contracts.Processes;

public sealed record OpenProcessInstanceRequest(
    Guid ProcessTypeId,
    Guid RequesterId,
    string Title,
    string Description,
    ProcessPriority Priority = ProcessPriority.Normal)
{
    public OpenProcessInstanceCommand ToCommand() => new(ProcessTypeId, RequesterId, Title, Description, Priority);
}
