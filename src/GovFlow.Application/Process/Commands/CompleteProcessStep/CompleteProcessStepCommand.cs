using MediatR;

namespace GovFlow.Application.Process.Commands.CompleteProcessStep;

/// <summary>
/// Completes the current step of a process instance, advancing the workflow (or resolving
/// the process when the last step is completed).
/// </summary>
public sealed record CompleteProcessStepCommand(
    Guid ProcessInstanceId,
    string? Notes = null) : IRequest<Unit>;
