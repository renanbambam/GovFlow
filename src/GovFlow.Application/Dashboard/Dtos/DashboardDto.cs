namespace GovFlow.Application.Dashboard.Dtos;

/// <summary>Executive summary counters for the dashboard.</summary>
public sealed record DashboardDto(
    int TotalOrganizations,
    int TotalProcessTypes,
    int TotalOpenProcesses,
    int TotalCompletedProcesses,
    int TotalProcesses);
