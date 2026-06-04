using GovFlow.Application.Process.Commands.DetectStalledProcesses;
using GovFlow.Application.Tests.Fakes;
using GovFlow.Domain.Process;
using GovFlow.Domain.Process.Enums;
using Xunit;

namespace GovFlow.Application.Tests.Process;

public class DetectStalledProcessesCommandHandlerTests
{
    private static ProcessInstance BuildOpenInstance()
    {
        var type = ProcessType.Create("p", "d", Guid.NewGuid());
        type.AddStep("s1", "first");
        return ProcessInstance.Open(type, Guid.NewGuid(), "t", "d");
    }

    [Fact]
    public async Task Flags_stalled_processes_and_records_breach_on_timeline()
    {
        var instances = new FakeProcessInstanceRepository();
        var stalled = BuildOpenInstance();
        instances.Items[stalled.Id] = stalled;
        instances.StalledIds.Add(stalled.Id);

        var handler = new DetectStalledProcessesCommandHandler(instances, new FakeUnitOfWork());

        var breached = await handler.Handle(new DetectStalledProcessesCommand(3), CancellationToken.None);

        Assert.Equal(1, breached);
        Assert.Contains(stalled.Timeline, e => e.EventType == ProcessEventType.SlaBreached);
    }

    [Fact]
    public async Task Does_not_breach_twice_without_new_activity()
    {
        var instances = new FakeProcessInstanceRepository();
        var stalled = BuildOpenInstance();
        instances.Items[stalled.Id] = stalled;
        instances.StalledIds.Add(stalled.Id);
        var handler = new DetectStalledProcessesCommandHandler(instances, new FakeUnitOfWork());

        await handler.Handle(new DetectStalledProcessesCommand(3), CancellationToken.None);
        var second = await handler.Handle(new DetectStalledProcessesCommand(3), CancellationToken.None);

        Assert.Equal(0, second);
        Assert.Single(stalled.Timeline, e => e.EventType == ProcessEventType.SlaBreached);
    }

    [Fact]
    public async Task Returns_zero_when_nothing_is_stalled()
    {
        var handler = new DetectStalledProcessesCommandHandler(new FakeProcessInstanceRepository(), new FakeUnitOfWork());

        var breached = await handler.Handle(new DetectStalledProcessesCommand(3), CancellationToken.None);

        Assert.Equal(0, breached);
    }
}
