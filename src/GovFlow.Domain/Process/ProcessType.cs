using GovFlow.Domain.Common;

namespace GovFlow.Domain.Process;

public sealed class ProcessType : AggregateRoot
{
    private readonly List<WorkflowStep> _steps = new();

    public string Name { get; private set; }

    public string Description { get; private set; }

    public Guid OrganizationId { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyList<WorkflowStep> Steps => _steps.AsReadOnly();

    private ProcessType(string name, string description, Guid organizationId)
    {
        Name = name;
        Description = description;
        OrganizationId = organizationId;
        IsActive = true;
    }

    public static ProcessType Create(string name, string description, Guid organizationId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Process type name is required.", nameof(name));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Process type must belong to an organization.", nameof(organizationId));

        return new ProcessType(name.Trim(), description?.Trim() ?? string.Empty, organizationId);
    }

    public WorkflowStep AddStep(
        string name,
        string description,
        Guid? assignableDepartmentId = null,
        int? slaHours = null,
        IEnumerable<string>? requiredDocuments = null)
    {
        var step = WorkflowStep.Create(
            Id,
            name,
            description,
            _steps.Count + 1,
            assignableDepartmentId,
            slaHours,
            requiredDocuments);

        _steps.Add(step);
        Touch();
        return step;
    }

    public void RemoveStep(Guid stepId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId)
            ?? throw new ArgumentException($"Step {stepId} does not belong to this process type.", nameof(stepId));

        _steps.Remove(step);
        Renumber();
        Touch();
    }

    public void ReorderSteps(IReadOnlyList<Guid> orderedStepIds)
    {
        if (orderedStepIds.Count != _steps.Count)
            throw new ArgumentException("Reordering must include every step exactly once.", nameof(orderedStepIds));

        var byId = _steps.ToDictionary(s => s.Id);
        var order = 1;
        foreach (var id in orderedStepIds)
        {
            if (!byId.TryGetValue(id, out var step))
                throw new ArgumentException($"Unknown step id {id}.", nameof(orderedStepIds));

            step.SetOrder(order++);
        }

        _steps.Sort((a, b) => a.Order.CompareTo(b.Order));
        Touch();
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Process type name is required.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Touch();
    }

    public void Archive()
    {
        if (!IsActive) return;

        IsActive = false;
        Touch();
    }

    private void Renumber()
    {
        var order = 1;
        foreach (var step in _steps.OrderBy(s => s.Order))
            step.SetOrder(order++);
    }
}
