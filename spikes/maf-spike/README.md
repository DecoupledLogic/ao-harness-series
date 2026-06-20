# maf-spike — Slice 0 (throwaway)

A disposable spike that exercises the Microsoft Agent Framework primitives end
to end against a real provider. **Not part of the v0 kernel.** Once Slice 1
(Hello Bounded Agent) lands and the kernel passes its first end-to-end demo,
this folder should be deleted or moved under `archive/`.

Do **not** borrow types, patterns, or namespaces from this spike into
`Harness.Core`. In the kernel, provider selection lives only in its own factory
and is deliberately re-implemented, not shared with this folder.

## What it proves

- `IChatClient` constructed from env-var-driven provider selection (Azure OpenAI
  preferred, OpenAI fallback)
- One toy function tool registered via `AIFunctionFactory.Create`
- A `ChatClientAgent` created with brief instructions and the tool
- A fresh `AgentSession` (the MAF 1.6.x replacement for `AgentThread`)
- One run against a hardcoded objective that provokes a tool call
- Tool invocation visible in the response messages
- Final text printed; exit code 0 on success, non-zero on assertion failure

## Run

Either provider works. The spike prefers Azure when all three Azure env vars
are set, else falls back to OpenAI, else exits with code 1 and a setup hint.

OpenAI:

```powershell
$env:OPENAI_API_KEY = "sk-..."
$env:OPENAI_MODEL   = "gpt-4o-mini"   # or any tool-capable model
dotnet run --project spikes/maf-spike
```

Azure OpenAI:

```powershell
$env:AZURE_OPENAI_ENDPOINT   = "https://<resource>.openai.azure.com/"
$env:AZURE_OPENAI_DEPLOYMENT = "<deployment-name>"
$env:AZURE_OPENAI_API_KEY    = "<key>"
dotnet run --project spikes/maf-spike
```

## Expected output

```
[spike] provider: OpenAI (model=gpt-4o-mini)
[spike] objective: What is the current time? Use your tool.

=== final response ===
The current time is 2026-05-18T....
======================
[spike] tool call observed: get_current_time (callId=...)
[spike] tool result observed: callId=... result=2026-05-18T...

[spike] total tool calls: 1
[spike] PASS
```
