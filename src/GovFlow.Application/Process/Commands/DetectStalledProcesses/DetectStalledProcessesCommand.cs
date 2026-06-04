using MediatR;

namespace GovFlow.Application.Process.Commands.DetectStalledProcesses;

public sealed record DetectStalledProcessesCommand(int IdleDays) : IRequest<int>;
