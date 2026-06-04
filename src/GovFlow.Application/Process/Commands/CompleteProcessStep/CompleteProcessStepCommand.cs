using MediatR;

namespace GovFlow.Application.Process.Commands.CompleteProcessStep;

public sealed record CompleteProcessStepCommand(
    Guid ProcessInstanceId,
    string? Notes = null) : IRequest<Unit>;
