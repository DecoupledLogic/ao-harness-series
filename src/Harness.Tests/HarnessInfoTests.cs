using Harness.Core;

namespace Harness.Tests;

public sealed class HarnessInfoTests
{
    [Fact]
    public void Describe_NamesTheHarness()
    {
        var description = HarnessInfo.Describe();

        Assert.Contains(HarnessInfo.Name, description);
    }

    [Fact]
    public void Stage_IsBootstrap()
    {
        Assert.Equal("bootstrap", HarnessInfo.Stage);
    }
}
