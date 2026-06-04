using GovFlow.Domain.Common;

namespace GovFlow.Domain.Organization;

public sealed class Department : AggregateRoot
{
    public string Name { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid? ParentDepartmentId { get; private set; }

    public Guid? ManagerUserId { get; private set; }

    public bool IsActive { get; private set; }

    private Department(string name, Guid organizationId, Guid? parentDepartmentId)
    {
        Name = name;
        OrganizationId = organizationId;
        ParentDepartmentId = parentDepartmentId;
        IsActive = true;
    }

    public static Department Create(string name, Guid organizationId, Guid? parentDepartmentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required.", nameof(name));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Department must belong to an organization.", nameof(organizationId));

        return new Department(name.Trim(), organizationId, parentDepartmentId);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required.", nameof(name));

        Name = name.Trim();
        Touch();
    }

    public void MoveTo(Guid? parentDepartmentId)
    {
        if (parentDepartmentId == Id)
            throw new InvalidOperationException("A department cannot be its own parent.");

        ParentDepartmentId = parentDepartmentId;
        Touch();
    }

    public void AssignManager(Guid managerUserId)
    {
        if (managerUserId == Guid.Empty)
            throw new ArgumentException("Manager user id is required.", nameof(managerUserId));

        ManagerUserId = managerUserId;
        Touch();
    }

    public void RemoveManager()
    {
        if (ManagerUserId is null) return;

        ManagerUserId = null;
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        Touch();
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        Touch();
    }
}
