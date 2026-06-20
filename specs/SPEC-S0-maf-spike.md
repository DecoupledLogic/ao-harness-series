# Spec: Slice 0 — Throwaway Microsoft Agent Framework Spike

**ID:** SPEC-S0
**Slice:** S0
**Source:** AgenticOps Harness v0 delivery plan and build spec (developed in the private product repo).

## User Story

As a developer, I want a disposable single-file spike that exercises Microsoft Agent Framework's `IChatClient` + `AIAgent` + `AgentSession` primitives end to end, so that I can confirm the framework runs locally before investing in the real harness structure.

> **Terminology note (added after delivery):** the spec originally referred to `AgentThread`. Microsoft Agent Framework 1.6.x renamed that primitive to `AgentSession` (created via `AIAgent.CreateSessionAsync()`). Same role — a fresh per-run conversation context — different name. AC and approach below use the current name.

## Acceptance Criteria

- [x] An `IChatClient` is constructed using the chosen provider (Azure OpenAI preferred, OpenAI fallback)
- [x] One toy function tool is registered via `AIFunctionFactory.Create`
- [x] An `AIAgent` / `ChatClientAgent` is created with brief instructions and the toy tool
- [x] A fresh `AgentSession` is created and the agent runs once against a hardcoded objective that should provoke a tool call
- [x] The toy tool is invoked at least once during the run (verified by tool result text appearing in the final response or trace)
- [x] The final response is printed to the console
- [x] Code lives outside the eventual `src/` tree and is clearly marked disposable (`spikes/maf-spike/` folder, README note, and top-of-file comment)

## Technical Approach

This slice intentionally does **not** belong in the final solution structure. It is built as a separate minimal project at `spikes/maf-spike/`:

- Single `.csproj` referencing the Microsoft Agent Framework NuGet packages
- `Program.cs` containing all spike code (no class library split, no folder hierarchy)
- Env-var-driven provider selection inline — mirrors the eventual provider factory rule but does not share code with it
- Toy tool: trivially small (`get_current_time`). Purpose is to verify tool dispatch, not deliver utility.

Do not borrow types, patterns, or namespaces from this spike into `Harness.Core` when Slice 1 begins. Once the v0 kernel passes its first end-to-end demo, this spike can be deleted or moved under an `archive/` folder.

## Testing Notes

Manual demo:

```bash
dotnet run --project spikes/maf-spike
```

Pass when:

- The console prints a non-empty agent final response
- The toy tool's result is visible (echoed value, timestamp, etc.)
- The process exits with code 0

No unit tests for this slice — it is a learning artifact, not protected code. The spike compiles as part of the build evidence for this release; running it end to end requires a provider key and is a manual demo.
