using GovFlow.Domain.Process;
using Xunit;

namespace GovFlow.Domain.Tests.Process;

public class ProcessTypeTests
{
    [Fact]
    public void AddStep_assigns_contiguous_order()
    {
        var processType = ProcessType.Create("p", "d", Guid.NewGuid());

        var first = processType.AddStep("a", "");
        var second = processType.AddStep("b", "");

        Assert.Equal(1, first.Order);
        Assert.Equal(2, second.Order);
    }

    [Fact]
    public void ReorderSteps_applies_the_provided_order()
    {
        var processType = ProcessType.Create("p", "d", Guid.NewGuid());
        var first = processType.AddStep("a", "");
        var second = processType.AddStep("b", "");

        processType.ReorderSteps(new[] { second.Id, first.Id });

        Assert.Equal(1, second.Order);
        Assert.Equal(2, first.Order);
    }

    [Fact]
    public void RemoveStep_renumbers_the_remaining_steps()
    {
        var processType = ProcessType.Create("p", "d", Guid.NewGuid());
        var first = processType.AddStep("a", "");
        var second = processType.AddStep("b", "");

        processType.RemoveStep(first.Id);

        Assert.Single(processType.Steps);
        Assert.Equal(1, second.Order);
    }
}
