// =============================================================================
// THROWAWAY SPIKE — DO NOT IMPORT INTO Harness.Core (Slice 0)
// =============================================================================
// Purpose: prove the Microsoft Agent Framework primitives (IChatClient,
// ChatClientAgent, AgentSession, AIFunctionFactory) run end-to-end against a
// real provider before we invest in the real harness structure.
//
// Disposable. Once Slice 1 (Hello Bounded Agent) lands and the v0 kernel
// passes its first end-to-end demo, delete or move this folder under archive/.
//
// NOTE: SPEC-S0 referred to AgentThread, but MAF 1.6.x renamed that concept
// to AgentSession (created via AIAgent.CreateSessionAsync). Same role: a
// fresh per-run conversation context. The AC's intent — a brand new
// conversation context, with one run against a hardcoded objective — is met.
// =============================================================================

using System.ClientModel;
using System.ComponentModel;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

// --- Env-var-driven provider selection (mirrors eventual ChatClientFactory rule;
//     does NOT share code with it — that's deliberate). -----------------------
var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var azureDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");
var azureApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var openAiModel = Environment.GetEnvironmentVariable("OPENAI_MODEL");

IChatClient chatClient;
string providerLabel;

if (!string.IsNullOrWhiteSpace(azureEndpoint)
    && !string.IsNullOrWhiteSpace(azureDeployment)
    && !string.IsNullOrWhiteSpace(azureApiKey))
{
    var azure = new AzureOpenAIClient(new Uri(azureEndpoint), new ApiKeyCredential(azureApiKey));
    chatClient = azure.GetChatClient(azureDeployment).AsIChatClient();
    providerLabel = $"Azure OpenAI (deployment={azureDeployment})";
}
else if (!string.IsNullOrWhiteSpace(openAiApiKey)
         && !string.IsNullOrWhiteSpace(openAiModel))
{
    var openAi = new OpenAIClient(openAiApiKey);
    chatClient = openAi.GetChatClient(openAiModel).AsIChatClient();
    providerLabel = $"OpenAI (model={openAiModel})";
}
else
{
    Console.Error.WriteLine(
        "No usable provider configured. Set EITHER:\n" +
        "  AZURE_OPENAI_ENDPOINT + AZURE_OPENAI_DEPLOYMENT + AZURE_OPENAI_API_KEY\n" +
        "OR:\n" +
        "  OPENAI_API_KEY + OPENAI_MODEL");
    return 1;
}

Console.WriteLine($"[spike] provider: {providerLabel}");

// --- Toy function tool — verifies tool dispatch, not utility. ----------------
[Description("Returns the current local time as an ISO-8601 string. Use when the user asks the current time.")]
static string GetCurrentTime() => DateTimeOffset.Now.ToString("o");

var getTimeTool = AIFunctionFactory.Create((Func<string>)GetCurrentTime, name: "get_current_time");

// --- Agent + session + single run. ------------------------------------------
AIAgent agent = new ChatClientAgent(
    chatClient,
    instructions:
        "You are a tiny spike agent. When the user asks for the current time, "
      + "you MUST call the get_current_time tool exactly once and incorporate "
      + "its returned ISO-8601 timestamp into a one-sentence reply.",
    tools: [getTimeTool]);

var session = await agent.CreateSessionAsync();
const string objective = "What is the current time? Use your tool.";
Console.WriteLine($"[spike] objective: {objective}");

var run = await agent.RunAsync(objective, session);

// --- Evidence: final response + a tally of tool calls observed. -------------
Console.WriteLine();
Console.WriteLine("=== final response ===");
Console.WriteLine(run.Text);
Console.WriteLine("======================");

var toolCallCount = 0;
foreach (var msg in run.Messages)
{
    foreach (var content in msg.Contents)
    {
        if (content is FunctionCallContent fc)
        {
            toolCallCount++;
            Console.WriteLine($"[spike] tool call observed: {fc.Name} (callId={fc.CallId})");
        }
        else if (content is FunctionResultContent fr)
        {
            Console.WriteLine($"[spike] tool result observed: callId={fr.CallId} result={fr.Result}");
        }
    }
}

Console.WriteLine();
Console.WriteLine($"[spike] total tool calls: {toolCallCount}");

if (toolCallCount == 0)
{
    Console.Error.WriteLine("[spike] FAIL: no tool call was observed — the spike's core assertion failed.");
    return 2;
}

if (string.IsNullOrWhiteSpace(run.Text))
{
    Console.Error.WriteLine("[spike] FAIL: agent produced no final text.");
    return 3;
}

Console.WriteLine("[spike] PASS");
return 0;
