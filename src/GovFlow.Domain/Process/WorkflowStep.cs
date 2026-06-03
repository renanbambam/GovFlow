using GovFlow.Domain.Common;

namespace GovFlow.Domain.Process;

/// <summary>
/// One ordered stage in a <see cref="ProcessType"/> template. Part of the ProcessType
/// aggregate, so it is only created and reordered through its parent.
/// </summary>
public sealed class WorkflowStep : Entity
{
    private readonly List<string> _requiredDocuments = new();

    public Guid ProcessTypeId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int Order { get; private set; }

    /// <summary>Department this step is assigned to by default, if any.</summary>
    public Guid? AssignableDepartmentId { get; private set; }

    /// <summary>Resolution target in hours, used later to derive an SLA policy.</summary>
    public int? SlaHours { get; private set; }

    public IReadOnlyList<string> RequiredDocuments => _requiredDocuments.AsReadOnly();

    private WorkflowStep(
        Guid processTypeId,
        string name,
        string description,
        int order,
        Guid? assignableDepartmentId,
        int? slaHours)
    {
        ProcessTypeId = processTypeId;
        Name = name;
        Description = description;
        Order = order;
        AssignableDepartmentId = assignableDepartmentId;
        SlaHours = slaHours;
    }

    internal static WorkflowStep Create(
        Guid processTypeId,
        string name,
        string description,
        int order,
        Guid? assignableDepartmentId,
        int? slaHours,
        IEnumerable<string>? requiredDocuments)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Step name is required.", nameof(name));
        if (slaHours is <= 0)
            throw new ArgumentException("SLA hours must be positive when provided.", nameof(slaHours));

        var step = new WorkflowStep(
            processTypeId,
            name.Trim(),
            description?.Trim() ?? string.Empty,
            order,
            assignableDepartmentId,
            slaHours);

        if (requiredDocuments is not null)
        {
            foreach (var document in requiredDocuments)
            {
                if (!string.IsNullOrWhiteSpace(document))
                    step._requiredDocuments.Add(document.Trim());
            }
        }

        return step;
    }

    internal void SetOrder(int order) => Order = order;
}
