using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Dashboard.Dtos;
using GovFlow.Application.Dashboard.Queries;
using GovFlow.Application.Organization.Dtos;
using GovFlow.Application.Organization.Queries;
using GovFlow.Application.Process.Dtos;
using GovFlow.Application.Process.Queries;
using GovFlow.Application.Tests.Fakes;
using Xunit;

namespace GovFlow.Application.Tests.Queries;

public class QueryHandlerTests
{
    [Fact]
    public async Task GetOrganizationById_returns_dto_when_found()
    {
        var repo = new FakeOrganizationReadRepository();
        var id = Guid.NewGuid();
        repo.Items.Add(new OrganizationDto(id, "Acme", "acme", true, DateTime.UtcNow));
        var handler = new GetOrganizationByIdQueryHandler(repo);

        var result = await handler.Handle(new GetOrganizationByIdQuery(id), CancellationToken.None);

        Assert.Equal("acme", result.Slug);
    }

    [Fact]
    public async Task GetOrganizationById_throws_not_found_when_missing()
    {
        var handler = new GetOrganizationByIdQueryHandler(new FakeOrganizationReadRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetOrganizationByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task GetOrganizations_returns_all()
    {
        var repo = new FakeOrganizationReadRepository();
        repo.Items.Add(new OrganizationDto(Guid.NewGuid(), "A", "a", true, DateTime.UtcNow));
        repo.Items.Add(new OrganizationDto(Guid.NewGuid(), "B", "b", true, DateTime.UtcNow));
        var handler = new GetOrganizationsQueryHandler(repo);

        var result = await handler.Handle(new GetOrganizationsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetDashboard_returns_counters()
    {
        var repo = new FakeDashboardReadRepository { Result = new DashboardDto(2, 3, 4, 5, 9) };
        var handler = new GetDashboardQueryHandler(repo);

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        Assert.Equal(9, result.TotalProcesses);
        Assert.Equal(4, result.TotalOpenProcesses);
    }

    [Fact]
    public async Task GetProcessTimeline_returns_entries_when_process_exists()
    {
        var repo = new FakeProcessReadRepository();
        var id = Guid.NewGuid();
        repo.Timelines[id] = new List<ProcessTimelineEntryDto>
        {
            new(Guid.NewGuid(), 1, "ProcessOpened", "opened", null, DateTime.UtcNow)
        };
        var handler = new GetProcessTimelineQueryHandler(repo);

        var result = await handler.Handle(new GetProcessTimelineQuery(id), CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetProcessTimeline_throws_not_found_when_process_missing()
    {
        var handler = new GetProcessTimelineQueryHandler(new FakeProcessReadRepository());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetProcessTimelineQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
