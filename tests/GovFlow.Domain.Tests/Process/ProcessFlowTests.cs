using GovFlow.Domain.Process;
using GovFlow.Domain.Process.Enums;
using Xunit;

namespace GovFlow.Domain.Tests.Process;

public class ProcessFlowTests
{
    private static ProcessType BuildTwoStepType()
    {
        var processType = ProcessType.Create("Onboarding", "desc", Guid.NewGuid());
        processType.AddStep("Step 1", "first");
        processType.AddStep("Step 2", "second");
        return processType;
    }

    [Fact]
    public void Open_starts_the_first_step()
    {
        var processType = BuildTwoStepType();

        var instance = ProcessInstance.Open(processType, Guid.NewGuid(), "Title", "Desc");

        Assert.Equal(ProcessStatus.Open, instance.Status);
        Assert.Equal(2, instance.Steps.Count);
        var current = instance.Steps.Single(s => s.Id == instance.CurrentStepId);
        Assert.Equal(1, current.Sequence);
        Assert.Equal(StepStatus.InProgress, current.Status);
    }

    [Fact]
    public void Completing_all_steps_resolves_the_process()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "Title", "Desc");

        instance.CompleteCurrentStep("done 1");
        Assert.Equal(ProcessStatus.InProgress, instance.Status);
        Assert.Equal(2, instance.Steps.Single(s => s.Id == instance.CurrentStepId).Sequence);

        instance.CompleteCurrentStep("done 2");

        Assert.Equal(ProcessStatus.Resolved, instance.Status);
        Assert.Null(instance.CurrentStepId);
        Assert.NotNull(instance.ClosedAt);
        Assert.All(instance.Steps, s => Assert.Equal(StepStatus.Completed, s.Status));
    }

    [Fact]
    public void Open_without_steps_throws()
    {
        var processType = ProcessType.Create("Empty", "d", Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(
            () => ProcessInstance.Open(processType, Guid.NewGuid(), "t", "d"));
    }

    [Fact]
    public void Cancelled_process_cannot_be_advanced()
    {
        var instance = ProcessInstance.Open(BuildTwoStepType(), Guid.NewGuid(), "t", "d");

        instance.Cancel();

        Assert.Equal(ProcessStatus.Cancelled, instance.Status);
        Assert.Throws<InvalidOperationException>(() => instance.CompleteCurrentStep());
    }
}
