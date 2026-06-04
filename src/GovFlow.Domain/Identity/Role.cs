using GovFlow.Domain.Common;

namespace GovFlow.Domain.Identity;

public sealed class Role : AggregateRoot
{
    private readonly HashSet<string> _permissionCodes = new(StringComparer.OrdinalIgnoreCase);

    public string Name { get; private set; }

    public Guid OrganizationId { get; private set; }

    public IReadOnlyCollection<string> PermissionCodes => _permissionCodes.ToArray();

    private Role(string name, Guid organizationId)
    {
        Name = name;
        OrganizationId = organizationId;
    }

    public static Role Create(string name, Guid organizationId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Role must belong to an organization.", nameof(organizationId));

        return new Role(name.Trim(), organizationId);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));

        Name = name.Trim();
        Touch();
    }

    public void Grant(string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
            throw new ArgumentException("Permission code is required.", nameof(permissionCode));

        if (_permissionCodes.Add(permissionCode.Trim()))
            Touch();
    }

    public void Revoke(string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode)) return;

        if (_permissionCodes.Remove(permissionCode.Trim()))
            Touch();
    }
}
