namespace Harness.Core;

/// <summary>
/// Minimal bootstrap surface for the public AgenticOps Harness series repo.
/// It exists so the solution has a real library to build, reference, and test
/// before the first slice lands. Slice 0 replaces this with real harness code.
/// </summary>
public static class HarnessInfo
{
    /// <summary>Name of the kernel this repo builds toward.</summary>
    public const string Name = "AgenticOps Harness";

    /// <summary>Current bootstrap stage. Slices replace this with real capability.</summary>
    public const string Stage = "bootstrap";

    /// <summary>One-line description the console prints to prove the wiring.</summary>
    public static string Describe() =>
        $"{Name} ({Stage}): clean build and test wired, ready for slice 0.";
}
