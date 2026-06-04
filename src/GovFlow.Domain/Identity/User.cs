using GovFlow.Domain.Common;

namespace GovFlow.Domain.Identity;

public sealed class User : AggregateRoot
{
    private readonly List<Guid> _roleIds = new();
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public Guid OrganizationId { get; private set; }

    public Guid? DepartmentId { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Guid> RoleIds => _roleIds.AsReadOnly();

    public IReadOnlyCollection<string> Roles => _roles.ToArray();

    private User(string name, string email, string passwordHash, Guid organizationId, Guid? departmentId)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        OrganizationId = organizationId;
        DepartmentId = departmentId;
        IsActive = true;
    }

    public static User Create(
        string name,
        string email,
        string passwordHash,
        Guid organizationId,
        Guid? departmentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("User must belong to an organization.", nameof(organizationId));

        return new User(name.Trim(), NormalizeEmail(email), passwordHash, organizationId, departmentId);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name is required.", nameof(name));

        Name = name.Trim();
        Touch();
    }

    public void ChangeEmail(string email)
    {
        Email = NormalizeEmail(email);
        Touch();
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        PasswordHash = passwordHash;
        Touch();
    }

    public void AssignToDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
            throw new ArgumentException("Department id is required.", nameof(departmentId));

        DepartmentId = departmentId;
        Touch();
    }

    public void RemoveFromDepartment()
    {
        if (DepartmentId is null) return;

        DepartmentId = null;
        Touch();
    }

    public void AssignRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role id is required.", nameof(roleId));
        if (_roleIds.Contains(roleId)) return;

        _roleIds.Add(roleId);
        Touch();
    }

    public void RevokeRole(Guid roleId)
    {
        if (_roleIds.Remove(roleId))
            Touch();
    }

    public void AssignRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name is required.", nameof(roleName));

        if (_roles.Add(roleName.Trim()))
            Touch();
    }

    public void RevokeRole(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName) && _roles.Remove(roleName.Trim()))
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

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("A valid email is required.", nameof(email));

        return email.Trim().ToLowerInvariant();
    }
}
