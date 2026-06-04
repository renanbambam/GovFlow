using GovFlow.Domain.Process;
using GovFlow.Domain.Process.Enums;
using Xunit;

namespace GovFlow.Domain.Tests.Process;

public class ProcessTimelineTests
{
    private static ProcessType BuildTwoStepType()
    {
        var processType = ProcessType.Create("Onboarding", "desc", Guid.NewGuid());
        processType.AddStep("Step 1", "first");
        processType.AddStep("Step 2", "second");
        return processType;
    }

    [Fact]
    public void Opening_records_opened_and_first_step_started()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "Title", "Desc");

        Assert.Collection(instance.Timeline,
            e => Assert.Equal(ProcessEventType.ProcessOpened, e.EventType),
            e => Assert.Equal(ProcessEventType.StepStarted, e.EventType));
        Assert.Equal(1, instance.Timeline[0].Sequence);
    }

    [Fact]
    public void Full_lifecycle_produces_chronological_timeline()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "Title", "Desc");

        instance.CompleteCurrentStep("done 1");
        instance.CompleteCurrentStep("done 2");

        var types = instance.Timeline.Select(e => e.EventType).ToList();
        Assert.Equal(new[]
        {
            ProcessEventType.ProcessOpened,
            ProcessEventType.StepStarted,
            ProcessEventType.StepCompleted,
            ProcessEventType.StepStarted,
            ProcessEventType.StepCompleted,
            ProcessEventType.ProcessResolved
        }, types);

        var sequences = instance.Timeline.Select(e => e.Sequence).ToList();
        Assert.Equal(Enumerable.Range(1, types.Count).ToList(), sequences);
    }

    [Fact]
    public void Cancelling_records_cancelled_entry()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "t", "d");

        instance.Cancel();

        Assert.Contains(instance.Timeline, e => e.EventType == ProcessEventType.ProcessCancelled);
    }

    [Fact]
    public void Sla_breach_is_recorded_once_per_idle_window()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "t", "d");

        Assert.True(instance.RegisterSlaBreach(5));
        Assert.False(instance.RegisterSlaBreach(5));
        Assert.Single(instance.Timeline, e => e.EventType == ProcessEventType.SlaBreached);
    }

    [Fact]
    public void Sla_breach_can_repeat_after_new_activity()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "t", "d");

        Assert.True(instance.RegisterSlaBreach(5));
        instance.CompleteCurrentStep("moved");
        Assert.True(instance.RegisterSlaBreach(5));

        Assert.Equal(2, instance.Timeline.Count(e => e.EventType == ProcessEventType.SlaBreached));
    }

    [Fact]
    public void Closed_process_does_not_register_sla_breach()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "t", "d");
        instance.Cancel();

        Assert.False(instance.RegisterSlaBreach(5));
    }
}
