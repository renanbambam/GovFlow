using GovFlow.Domain.Process.Enums;
using MediatR;

namespace GovFlow.Application.Process.Commands.OpenProcessInstance;

public sealed record OpenProcessInstanceCommand(
    Guid ProcessTypeId,
    Guid RequesterId,
    string Title,
    string Description,
    ProcessPriority Priority = ProcessPriority.Normal) : IRequest<Guid>;
