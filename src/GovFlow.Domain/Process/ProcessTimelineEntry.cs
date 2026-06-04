using GovFlow.Domain.Common;
using GovFlow.Domain.Process.Enums;

namespace GovFlow.Domain.Process;

public sealed class ProcessTimelineEntry : Entity
{
    public Guid ProcessInstanceId { get; private set; }

    public int Sequence { get; private set; }

    public ProcessEventType EventType { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public Guid? StepId { get; private set; }

    public DateTime OccurredAt { get; private set; }

    private ProcessTimelineEntry()
    {
    }

    internal ProcessTimelineEntry(
        Guid processInstanceId,
        int sequence,
        ProcessEventType eventType,
        string description,
        Guid? stepId)
    {
        ProcessInstanceId = processInstanceId;
        Sequence = sequence;
        EventType = eventType;
        Description = description;
        StepId = stepId;
        OccurredAt = DateTime.UtcNow;
    }
}
