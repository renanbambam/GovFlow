using GovFlow.Domain.Common;
using GovFlow.Domain.Organization;
using GovFlow.Domain.Process;
using OrganizationAggregate = GovFlow.Domain.Organization.Organization;

namespace GovFlow.Application.Tests.Fakes;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}

internal sealed class FakeOrganizationRepository : IOrganizationRepository
{
    public Dictionary<Guid, OrganizationAggregate> Items { get; } = new();

    public Task<OrganizationAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.TryGetValue(id, out var value) ? value : null);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Values.Any(o => o.Slug == slug));

    public Task AddAsync(OrganizationAggregate organization, CancellationToken cancellationToken = default)
    {
        Items[organization.Id] = organization;
        return Task.CompletedTask;
    }
}

internal sealed class FakeProcessTypeRepository : IProcessTypeRepository
{
    public Dictionary<Guid, ProcessType> Items { get; } = new();

    public Task<ProcessType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.TryGetValue(id, out var value) ? value : null);

    public Task AddAsync(ProcessType processType, CancellationToken cancellationToken = default)
    {
        Items[processType.Id] = processType;
        return Task.CompletedTask;
    }
}

internal sealed class FakeProcessInstanceRepository : IProcessInstanceRepository
{
    public Dictionary<Guid, ProcessInstance> Items { get; } = new();

    public Task<ProcessInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.TryGetValue(id, out var value) ? value : null);

    public Task AddAsync(ProcessInstance instance, CancellationToken cancellationToken = default)
    {
        Items[instance.Id] = instance;
        return Task.CompletedTask;
    }
}
