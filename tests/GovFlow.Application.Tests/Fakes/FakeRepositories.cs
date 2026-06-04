using GovFlow.Application.Common.Interfaces;
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

internal sealed class FakeProcessRealtimeNotifier : IProcessRealtimeNotifier
{
    public List<(Guid ProcessId, string Status)> Notifications { get; } = new();

    public Task ProcessStatusChangedAsync(Guid processId, string status, CancellationToken cancellationToken = default)
    {
        Notifications.Add((processId, status));
        return Task.CompletedTask;
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

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.ContainsKey(id));

    public List<Guid> StalledIds { get; } = new();

    public Task<IReadOnlyList<Guid>> GetStalledProcessIdsAsync(DateTime threshold, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Guid>>(StalledIds);

    public Task AddAsync(ProcessInstance instance, CancellationToken cancellationToken = default)
    {
        Items[instance.Id] = instance;
        return Task.CompletedTask;
    }
}

internal sealed class FakeProcessCommentRepository : IProcessCommentRepository
{
    public List<ProcessComment> Items { get; } = new();

    public Task AddAsync(ProcessComment comment, CancellationToken cancellationToken = default)
    {
        Items.Add(comment);
        return Task.CompletedTask;
    }
}

internal sealed class FakeProcessDocumentRepository : IProcessDocumentRepository
{
    public List<ProcessDocument> Items { get; } = new();

    public Task AddAsync(ProcessDocument document, CancellationToken cancellationToken = default)
    {
        Items.Add(document);
        return Task.CompletedTask;
    }
}
