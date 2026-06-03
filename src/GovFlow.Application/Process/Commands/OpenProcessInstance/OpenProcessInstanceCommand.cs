using GovFlow.Domain.Process.Enums;
using MediatR;

namespace GovFlow.Application.Process.Commands.OpenProcessInstance;

/// <summary>Opens a running process instance from a process type. Returns the new instance id.</summary>
public sealed record OpenProcessInstanceCommand(
    Guid ProcessTypeId,
    Guid RequesterId,
    string Title,
    string Description,
    ProcessPriority Priority = ProcessPriority.Normal) : IRequest<Guid>;
