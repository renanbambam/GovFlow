namespace GovFlow.Application.Dashboard.Dtos;

public sealed record DashboardDto(
    int TotalOrganizations,
    int TotalProcessTypes,
    int TotalOpenProcesses,
    int TotalCompletedProcesses,
    int TotalProcesses);
