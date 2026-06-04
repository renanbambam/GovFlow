using GovFlow.Domain.Common;
using GovFlow.Domain.Process.Enums;
using GovFlow.Domain.Process.Events;

namespace GovFlow.Domain.Process;

public sealed class ProcessInstance : AggregateRoot
{
    private readonly List<ProcessInstanceStep> _steps = new();
    private readonly List<ProcessTimelineEntry> _timeline = new();

    public Guid ProcessTypeId { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Guid RequesterId { get; private set; }

    public Guid? CurrentStepId { get; private set; }

    public ProcessStatus Status { get; private set; }

    public ProcessPriority Priority { get; private set; }

    public DateTime OpenedAt { get; private set; }

    public DateTime? ClosedAt { get; private set; }

    public DateTime? DueAt { get; private set; }

    public IReadOnlyList<ProcessInstanceStep> Steps => _steps.AsReadOnly();

    public IReadOnlyList<ProcessTimelineEntry> Timeline => _timeline.AsReadOnly();

    private ProcessInstance(
        Guid processTypeId,
        Guid organizationId,
        string title,
        string description,
        Guid requesterId,
        ProcessPriority priority)
    {
        ProcessTypeId = processTypeId;
        OrganizationId = organizationId;
        Title = title;
        Description = description;
        RequesterId = requesterId;
        Priority = priority;
        Status = ProcessStatus.Draft;
        OpenedAt = DateTime.UtcNow;
    }

    public static ProcessInstance Open(
        ProcessType processType,
        Guid requesterId,
        string title,
        string description,
        ProcessPriority priority = ProcessPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(processType);
        if (!processType.IsActive)
            throw new InvalidOperationException("Cannot open a process from an archived process type.");
        if (processType.Steps.Count == 0)
            throw new InvalidOperationException("The process type has no workflow steps.");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Process title is required.", nameof(title));
        if (requesterId == Guid.Empty)
            throw new ArgumentException("Requester id is required.", nameof(requesterId));

        var instance = new ProcessInstance(
            processType.Id,
            processType.OrganizationId,
            title.Trim(),
            description?.Trim() ?? string.Empty,
            requesterId,
            priority);

        foreach (var workflowStep in processType.Steps.OrderBy(s => s.Order))
        {
            var step = ProcessInstanceStep.Create(
                instance.Id,
                workflowStep.Id,
                workflowStep.Order,
                workflowStep.AssignableDepartmentId);
            instance._steps.Add(step);
        }

        var first = instance._steps.OrderBy(s => s.Sequence).First();
        first.Start();
        instance.CurrentStepId = first.Id;
        instance.Status = ProcessStatus.Open;

        instance.AddTimelineEntry(ProcessEventType.ProcessOpened, $"Process \"{instance.Title}\" was opened.");
        instance.AddTimelineEntry(ProcessEventType.StepStarted, $"Step {first.Sequence} started.", first.Id);

        instance.RaiseDomainEvent(new ProcessInstanceOpenedEvent(instance.Id, instance.ProcessTypeId, requesterId));
        return instance;
    }

    public void AssignCurrentStepToUser(Guid userId)
    {
        EnsureActive();
        CurrentStepOrThrow().AssignToUser(userId);
        Touch();
    }

    public void AssignCurrentStepToDepartment(Guid departmentId)
    {
        EnsureActive();
        CurrentStepOrThrow().AssignToDepartment(departmentId);
        Touch();
    }

    public void CompleteCurrentStep(string? notes = null)
    {
        EnsureActive();
        var current = CurrentStepOrThrow();
        current.Complete(notes);
        AddTimelineEntry(ProcessEventType.StepCompleted, $"Step {current.Sequence} completed.", current.Id);
        RaiseDomainEvent(new ProcessStepCompletedEvent(Id, current.Id));

        var next = NextStep(current);
        if (next is null)
        {
            Resolve();
            return;
        }

        next.Start();
        CurrentStepId = next.Id;
        Status = ProcessStatus.InProgress;
        AddTimelineEntry(ProcessEventType.StepStarted, $"Step {next.Sequence} started.", next.Id);
        Touch();
    }

    public void ReturnCurrentStep(string? notes = null)
    {
        EnsureActive();
        var current = CurrentStepOrThrow();
        var previous = PreviousStep(current);
        if (previous is null)
            throw new InvalidOperationException("The first step cannot be returned.");

        current.Return(notes);
        previous.Start();
        CurrentStepId = previous.Id;
        Status = ProcessStatus.InProgress;
        AddTimelineEntry(ProcessEventType.StepReturned, $"Step {current.Sequence} returned to step {previous.Sequence}.", current.Id);
        AddTimelineEntry(ProcessEventType.StepStarted, $"Step {previous.Sequence} started.", previous.Id);
        RaiseDomainEvent(new ProcessStepReturnedEvent(Id, current.Id));
        Touch();
    }

    public void PutOnHold()
    {
        if (Status is not (ProcessStatus.Open or ProcessStatus.InProgress))
            throw new InvalidOperationException("Only an active process can be put on hold.");

        Status = ProcessStatus.OnHold;
        AddTimelineEntry(ProcessEventType.ProcessOnHold, "Process put on hold.");
        Touch();
    }

    public void Resume()
    {
        if (Status != ProcessStatus.OnHold)
            throw new InvalidOperationException("Only a process on hold can be resumed.");

        Status = ProcessStatus.InProgress;
        AddTimelineEntry(ProcessEventType.ProcessResumed, "Process resumed.");
        Touch();
    }

    public void Resolve()
    {
        EnsureActive();
        Status = ProcessStatus.Resolved;
        ClosedAt = DateTime.UtcNow;
        CurrentStepId = null;
        AddTimelineEntry(ProcessEventType.ProcessResolved, "Process resolved.");
        RaiseDomainEvent(new ProcessInstanceResolvedEvent(Id));
        Touch();
    }

    public void Cancel()
    {
        if (IsClosed())
            throw new InvalidOperationException("The process is already closed.");

        Status = ProcessStatus.Cancelled;
        ClosedAt = DateTime.UtcNow;
        CurrentStepId = null;
        AddTimelineEntry(ProcessEventType.ProcessCancelled, "Process cancelled.");
        RaiseDomainEvent(new ProcessInstanceCancelledEvent(Id));
        Touch();
    }

    public void ChangePriority(ProcessPriority priority)
    {
        if (Priority == priority) return;

        Priority = priority;
        Touch();
    }

    public void SetDueDate(DateTime dueAtUtc)
    {
        if (dueAtUtc <= OpenedAt)
            throw new ArgumentException("Due date must be after the process was opened.", nameof(dueAtUtc));

        DueAt = dueAtUtc;
        Touch();
    }

    public bool RegisterSlaBreach(int idleDays)
    {
        if (IsClosed())
            return false;

        var lastActivitySeq = _timeline
            .Where(e => e.EventType != ProcessEventType.SlaBreached)
            .Select(e => (int?)e.Sequence)
            .Max() ?? 0;
        var lastBreachSeq = _timeline
            .Where(e => e.EventType == ProcessEventType.SlaBreached)
            .Select(e => (int?)e.Sequence)
            .Max() ?? 0;
        if (lastBreachSeq > lastActivitySeq)
            return false;

        AddTimelineEntry(
            ProcessEventType.SlaBreached,
            $"SLA breach: no activity for {idleDays} day(s).");
        Touch();
        return true;
    }

    private void AddTimelineEntry(ProcessEventType eventType, string description, Guid? stepId = null)
        => _timeline.Add(new ProcessTimelineEntry(Id, _timeline.Count + 1, eventType, description, stepId));

    private ProcessInstanceStep CurrentStepOrThrow()
        => _steps.FirstOrDefault(s => s.Id == CurrentStepId)
           ?? throw new InvalidOperationException("The process has no current step.");

    private ProcessInstanceStep? NextStep(ProcessInstanceStep current)
        => _steps.Where(s => s.Sequence > current.Sequence)
                 .OrderBy(s => s.Sequence)
                 .FirstOrDefault();

    private ProcessInstanceStep? PreviousStep(ProcessInstanceStep current)
        => _steps.Where(s => s.Sequence < current.Sequence)
                 .OrderByDescending(s => s.Sequence)
                 .FirstOrDefault();

    private bool IsClosed()
        => Status is ProcessStatus.Resolved or ProcessStatus.Cancelled or ProcessStatus.Rejected;

    private void EnsureActive()
    {
        if (IsClosed())
            throw new InvalidOperationException("The process is closed and cannot be modified.");
    }
}
