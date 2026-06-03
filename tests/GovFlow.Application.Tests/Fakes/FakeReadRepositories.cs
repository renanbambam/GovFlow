using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Dashboard.Dtos;
using GovFlow.Application.Organization.Dtos;
using GovFlow.Application.Process.Dtos;

namespace GovFlow.Application.Tests.Fakes;

internal sealed class FakeOrganizationReadRepository : IOrganizationReadRepository
{
    public List<OrganizationDto> Items { get; } = new();

    public Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(o => o.Id == id));

    public Task<IReadOnlyList<OrganizationDto>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<OrganizationDto>>(Items);
}

internal sealed class FakeProcessTypeReadRepository : IProcessTypeReadRepository
{
    public List<ProcessTypeDto> Items { get; } = new();

    public Task<ProcessTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<ProcessTypeDto>> ListAsync(Guid? organizationId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ProcessTypeDto>>(
            Items.Where(p => organizationId == null || p.OrganizationId == organizationId).ToList());
}

internal sealed class FakeProcessReadRepository : IProcessReadRepository
{
    public List<ProcessInstanceDto> Items { get; } = new();

    public Task<ProcessInstanceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<ProcessSummaryDto>> ListAsync(Guid? organizationId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ProcessSummaryDto>>(
            Items
                .Where(p => organizationId == null || p.OrganizationId == organizationId)
                .Select(p => new ProcessSummaryDto(p.Id, p.ProcessTypeId, p.OrganizationId, p.Title, p.Status, p.Priority, p.OpenedAt, p.ClosedAt))
                .ToList());
}

internal sealed class FakeDashboardReadRepository : IDashboardReadRepository
{
    public DashboardDto Result { get; set; } = new(0, 0, 0, 0, 0);

    public Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(Result);
}
