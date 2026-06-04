using GovFlow.Domain.Common;

namespace GovFlow.Domain.Organization;

public sealed class Organization : AggregateRoot
{
    public string Name { get; private set; }

    public string Slug { get; private set; }

    public bool IsActive { get; private set; }

    private Organization(string name, string slug)
    {
        Name = name;
        Slug = slug;
        IsActive = true;
    }

    public static Organization Create(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Organization slug is required.", nameof(slug));

        return new Organization(name.Trim(), slug.Trim().ToLowerInvariant());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));

        Name = name.Trim();
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
