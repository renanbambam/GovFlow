namespace GovFlow.Domain.Process.Enums;

/// <summary>Lifecycle state of a <see cref="ProcessInstance"/>.</summary>
public enum ProcessStatus
{
    Draft = 0,
    Open = 1,
    InProgress = 2,
    OnHold = 3,
    Resolved = 4,
    Cancelled = 5,
    Rejected = 6
}
