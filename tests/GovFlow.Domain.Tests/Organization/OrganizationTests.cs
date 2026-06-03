using Xunit;
using OrganizationAggregate = GovFlow.Domain.Organization.Organization;

namespace GovFlow.Domain.Tests.Organization;

public class OrganizationTests
{
    [Fact]
    public void Create_normalizes_the_slug()
    {
        var organization = OrganizationAggregate.Create("City Hall", "City-Hall");

        Assert.Equal("city-hall", organization.Slug);
        Assert.True(organization.IsActive);
    }

    [Fact]
    public void Create_with_empty_name_throws()
    {
        Assert.Throws<ArgumentException>(() => OrganizationAggregate.Create(" ", "slug"));
    }

    [Fact]
    public void Deactivate_marks_inactive()
    {
        var organization = OrganizationAggregate.Create("Acme", "acme");

        organization.Deactivate();

        Assert.False(organization.IsActive);
    }
}
