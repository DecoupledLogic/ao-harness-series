# AgenticOps Harness Series

The public, build-in-public companion to the **AgenticOps Harness** blog series. The series builds an AI Engineering kernel one vertical slice at a time and publishes the build log next to the code. This repository is that code.

The hardened product is developed in a separate private repository. Curated code is moved here to lead each post, so what you read is backed by a real commit you can check out, build, and test. Each post pins to a tagged release that marks the exact state it describes.

This first commit is the bootstrap: a README and just enough code for a clean build and a green test, proving the toolchain is wired before slice 0 is written against it.

## What's here

```
Harness.slnx              solution: src/ Console, Core, Tests
src/Harness.Core          the library the slices grow into
src/Harness.Console       CLI entry point
src/Harness.Tests         xUnit tests
spikes/maf-spike          Slice 0: disposable Microsoft Agent Framework spike
specs/                    per-slice specs and acceptance criteria
docs/releases/            per-tag release notes (evidence)
.github/workflows/ci.yml  build + test on every push and PR
```

## Build and test

Requires the .NET 9 SDK.

```bash
dotnet restore Harness.slnx
dotnet build Harness.slnx
dotnet test Harness.slnx
dotnet run --project src/Harness.Console
```

The console prints one line proving the Core library is referenced and running. The tests prove the runner is wired. That is the whole point of the bootstrap: nothing more, and nothing the first slice has to undo.

## The ladder

The series climbs a ladder of control instincts for stochastic systems, rung by rung, each proven by a slice in this repo. Start with the anchor post, *The AI Engineering Maturity Ladder*, then follow the build log.

## Status

| Stage | State |
|-------|-------|
| Bootstrap (clean build + test) | done |
| Slice 0: disposable MAF spike | shipped, v0.1.0 |
| Slice 1: hello bounded agent | next |

Releases are tagged to align with published posts.

## License

Apache License 2.0. See [LICENSE](LICENSE).
