namespace GovFlow.Domain.Process.Enums;

/// <summary>State of a single <see cref="ProcessInstanceStep"/> within a running process.</summary>
public enum StepStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Skipped = 3,
    Returned = 4
}
