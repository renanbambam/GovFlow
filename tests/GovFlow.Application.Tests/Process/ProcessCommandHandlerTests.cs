using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Process.Commands.CompleteProcessStep;
using GovFlow.Application.Process.Commands.OpenProcessInstance;
using GovFlow.Application.Tests.Fakes;
using GovFlow.Domain.Process;
using GovFlow.Domain.Process.Enums;
using Xunit;

namespace GovFlow.Application.Tests.Process;

public class ProcessCommandHandlerTests
{
    private static ProcessType BuildType()
    {
        var processType = ProcessType.Create("p", "d", Guid.NewGuid());
        processType.AddStep("s1", "first");
        return processType;
    }

    [Fact]
    public async Task Open_creates_instance_from_type()
    {
        var typeRepo = new FakeProcessTypeRepository();
        var instanceRepo = new FakeProcessInstanceRepository();
        var processType = BuildType();
        typeRepo.Items[processType.Id] = processType;

        var notifier = new FakeProcessRealtimeNotifier();
        var handler = new OpenProcessInstanceCommandHandler(typeRepo, instanceRepo, notifier, new FakeUnitOfWork());

        var id = await handler.Handle(
            new OpenProcessInstanceCommand(processType.Id, Guid.NewGuid(), "Title", "Desc", ProcessPriority.High),
            CancellationToken.None);

        Assert.True(instanceRepo.Items.ContainsKey(id));
        Assert.Contains(notifier.Notifications, n => n.ProcessId == id && n.Status == "Open");
    }

    [Fact]
    public async Task Open_with_unknown_type_throws_not_found()
    {
        var handler = new OpenProcessInstanceCommandHandler(
            new FakeProcessTypeRepository(),
            new FakeProcessInstanceRepository(),
            new FakeProcessRealtimeNotifier(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(
                new OpenProcessInstanceCommand(Guid.NewGuid(), Guid.NewGuid(), "t", "d"),
                CancellationToken.None));
    }

    [Fact]
    public async Task Complete_step_resolves_single_step_process()
    {
        var instanceRepo = new FakeProcessInstanceRepository();
        var instance = ProcessInstance.Open(BuildType(), Guid.NewGuid(), "t", "d");
        instanceRepo.Items[instance.Id] = instance;

        var notifier = new FakeProcessRealtimeNotifier();
        var handler = new CompleteProcessStepCommandHandler(instanceRepo, notifier, new FakeUnitOfWork());

        await handler.Handle(new CompleteProcessStepCommand(instance.Id, "ok"), CancellationToken.None);

        Assert.Equal(ProcessStatus.Resolved, instance.Status);
        Assert.Contains(notifier.Notifications, n => n.ProcessId == instance.Id && n.Status == "Resolved");
    }

    [Fact]
    public async Task Complete_step_with_unknown_instance_throws_not_found()
    {
        var handler = new CompleteProcessStepCommandHandler(
            new FakeProcessInstanceRepository(), new FakeProcessRealtimeNotifier(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new CompleteProcessStepCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
