using GovFlow.Application.Process.Commands.CompleteProcessStep;

namespace GovFlow.API.Contracts.Processes;

/// <summary>Payload to complete the current step of a process instance.</summary>
/// <param name="Notes">Optional notes recorded on the completed step.</param>
public sealed record CompleteProcessStepRequest(string? Notes = null)
{
    public CompleteProcessStepCommand ToCommand(Guid processInstanceId) => new(processInstanceId, Notes);
}
