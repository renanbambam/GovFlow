using GovFlow.Application.Process.Commands.CompleteProcessStep;

namespace GovFlow.API.Contracts.Processes;

public sealed record CompleteProcessStepRequest(string? Notes = null)
{
    public CompleteProcessStepCommand ToCommand(Guid processInstanceId) => new(processInstanceId, Notes);
}
