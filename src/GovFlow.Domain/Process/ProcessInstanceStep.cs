using GovFlow.Domain.Common;
using GovFlow.Domain.Process.Enums;

namespace GovFlow.Domain.Process;

/// <summary>
/// A materialized step of a running <see cref="ProcessInstance"/>, snapshotted from the
/// corresponding <see cref="WorkflowStep"/> when the process is opened. Part of the
/// ProcessInstance aggregate, so its transitions are driven by the parent.
/// </summary>
public sealed class ProcessInstanceStep : Entity
{
    public Guid ProcessInstanceId { get; private set; }

    public Guid WorkflowStepId { get; private set; }

    /// <summary>1-based position in the workflow, mirrored from the originating step so the
    /// running order survives reloads from persistence.</summary>
    public int Sequence { get; private set; }

    public Guid? AssignedUserId { get; private set; }

    public Guid? AssignedDepartmentId { get; private set; }

    public StepStatus Status { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public string? Notes { get; private set; }

    private ProcessInstanceStep(Guid processInstanceId, Guid workflowStepId, int sequence, Guid? assignedDepartmentId)
    {
        ProcessInstanceId = processInstanceId;
        WorkflowStepId = workflowStepId;
        Sequence = sequence;
        AssignedDepartmentId = assignedDepartmentId;
        Status = StepStatus.Pending;
    }

    internal static ProcessInstanceStep Create(
        Guid processInstanceId,
        Guid workflowStepId,
        int sequence,
        Guid? assignedDepartmentId)
        => new(processInstanceId, workflowStepId, sequence, assignedDepartmentId);

    internal void Start()
    {
        Status = StepStatus.InProgress;
        StartedAt ??= DateTime.UtcNow;
        CompletedAt = null;
    }

    internal void AssignToUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));

        AssignedUserId = userId;
    }

    internal void AssignToDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            throw new ArgumentException("Department id is required.", nameof(departmentId));

        AssignedDepartmentId = departmentId;
    }

    internal void Complete(string? notes)
    {
        Status = StepStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes))
            Notes = notes.Trim();
    }

    internal void Return(string? notes)
    {
        Status = StepStatus.Returned;
        CompletedAt = null;
        if (!string.IsNullOrWhiteSpace(notes))
            Notes = notes.Trim();
    }
}
