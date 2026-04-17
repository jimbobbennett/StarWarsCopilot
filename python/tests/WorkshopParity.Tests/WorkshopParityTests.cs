using System.Text.RegularExpressions;

namespace WorkshopParity.Tests;

public class WorkshopParityTests
{
    private static readonly string RepoRoot = GetRepoRoot();

    private static readonly string[] StepRoots =
    [
        "1-chat-with-copilot",
        "2-chat-history-and-message-roles",
        "3-llm-choice",
        "4-call-tools",
        "5-mcp",
        "6-rag",
        "7-multimodal",
        "8-agents"
    ];

    [Fact]
    public void All_workshop_steps_have_after_readme_and_solution()
    {
        foreach (var step in StepRoots)
        {
            var stepPath = Path.Combine(RepoRoot, step);
            Assert.True(Directory.Exists(stepPath), $"Missing step folder: {step}");
            var readmeExists = Directory
                .GetFiles(stepPath, "*", SearchOption.TopDirectoryOnly)
                .Any(file => Path.GetFileName(file).Equals("README.md", StringComparison.OrdinalIgnoreCase));
            Assert.True(readmeExists, $"Missing README for {step}");
            Assert.True(Directory.Exists(Path.Combine(stepPath, "after")), $"Missing after folder for {step}");
        }
    }

    [Theory]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/Program.cs", "while (true)")]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/Program.cs", "GetResponseAsync(userInput)")]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/Program.cs", "Console.Write(\"User > \")")]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/Program.cs", "Console.WriteLine(\"Assistant > \" + result.Messages.Last()?.Text)")]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/LLMOptions.cs", "OpenAI:Endpoint")]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/LLMOptions.cs", "OpenAI:ApiKey")]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/LLMOptions.cs", "OpenAI:ModelName")]
    [InlineData("2-chat-history-and-message-roles/after/StarWarsCopilot/Program.cs", "new List<ChatMessage>")]
    [InlineData("2-chat-history-and-message-roles/after/StarWarsCopilot/Program.cs", "new(ChatRole.System")]
    [InlineData("2-chat-history-and-message-roles/after/StarWarsCopilot/Program.cs", "history.Add(new ChatMessage(ChatRole.User, userInput))")]
    [InlineData("2-chat-history-and-message-roles/after/StarWarsCopilot/Program.cs", "GetResponseAsync(history)")]
    [InlineData("2-chat-history-and-message-roles/after/StarWarsCopilot/Program.cs", "history.Add(new ChatMessage(ChatRole.Assistant, result.Messages.Last()?.Text ?? string.Empty))")]
    [InlineData("3-llm-choice/after/StarWarsCopilot/Program.cs", "FoundryLocalManager.StartModelAsync")]
    [InlineData("3-llm-choice/after/StarWarsCopilot/Program.cs", "openAIClient.GetChatClient(model!.ModelId).AsIChatClient()")]
    [InlineData("3-llm-choice/after/StarWarsCopilot/Program.cs", "ChatCompletionsClient")]
    [InlineData("3-llm-choice/after/StarWarsCopilot/LLMOptions.cs", "AIInference:Endpoint")]
    [InlineData("3-llm-choice/after/StarWarsCopilot/LLMOptions.cs", "AIInference:ModelName")]
    [InlineData("4-call-tools/after/StarWarsCopilot/Program.cs", "UseFunctionInvocation()")]
    [InlineData("4-call-tools/after/StarWarsCopilot/Program.cs", "new WookiepediaTool(ToolsOptions.TavilyApiKey)")]
    [InlineData("4-call-tools/after/StarWarsCopilot/Program.cs", "ChatOptions options = new() { Tools = tools }")]
    [InlineData("4-call-tools/after/StarWarsCopilot/Program.cs", "GetResponseAsync(history, options)")]
    [InlineData("4-call-tools/after/StarWarsCopilot/WookiepediaTool.cs", "public override string Name => \"WookiepediaTool\";")]
    [InlineData("4-call-tools/after/StarWarsCopilot/WookiepediaTool.cs", "include_domains = new[] { \"https://starwars.fandom.com/\" }")]
    [InlineData("4-call-tools/after/StarWarsCopilot/ToolsOptions.cs", "Tavily:ApiKey")]
    [InlineData("5-mcp/after/StarWarsMCPServer/Program.cs", ".AddMcpServer()")]
    [InlineData("5-mcp/after/StarWarsMCPServer/Program.cs", ".WithStdioServerTransport()")]
    [InlineData("5-mcp/after/StarWarsMCPServer/Program.cs", ".WithToolsFromAssembly()")]
    [InlineData("5-mcp/after/StarWarsMCPServer/StarWarsTools.cs", "[McpServerTool(Name = \"WookiepediaTool\")")]
    [InlineData("5-mcp/after/StarWarsCopilot/Program.cs", "new StdioClientTransport")]
    [InlineData("5-mcp/after/StarWarsCopilot/Program.cs", "McpClient.CreateAsync")]
    [InlineData("5-mcp/after/StarWarsCopilot/Program.cs", "await mcpClient.ListToolsAsync()")]
    [InlineData("6-rag/after/StarWarsMCPServer/StarWarsTools.cs", "[McpServerTool(Name = \"StarWarsPurchaseTool\")")]
    [InlineData("6-rag/after/StarWarsMCPServer/StarWarsTools.cs", "new TableServiceClient(ToolsOptions.AzureStorageConnectionString)")]
    [InlineData("6-rag/after/StarWarsMCPServer/StarWarsTools.cs", "At least one parameter is required")]
    [InlineData("6-rag/after/StarWarsMCPServer/ToolsOptions.cs", "AzureStorage:ConnectionString")]
    [InlineData("7-multimodal/after/StarWarsMCPServer/StarWarsTools.cs", "[McpServerTool(Name = \"GenerateStarWarsImageTool\")")]
    [InlineData("7-multimodal/after/StarWarsMCPServer/StarWarsTools.cs", "Description cannot be empty.")]
    [InlineData("7-multimodal/after/StarWarsMCPServer/StarWarsTools.cs", "content_policy_violation")]
    [InlineData("7-multimodal/after/StarWarsMCPServer/StarWarsTools.cs", "Please retry this tool with an adjusted prompt")]
    [InlineData("7-multimodal/after/StarWarsMCPServer/ToolsOptions.cs", "ImageGeneration:Endpoint")]
    [InlineData("7-multimodal/after/StarWarsMCPServer/ToolsOptions.cs", "ImageGeneration:ApiKey")]
    [InlineData("7-multimodal/after/StarWarsMCPServer/ToolsOptions.cs", "ImageGeneration:ModelName")]
    [InlineData("7-multimodal/after/StarWarsCopilot/Program.cs", "If a tool responds asking you to call it again, follow the instructions and call the tool again.")]
    [InlineData("8-agents/after/StarWarsCopilot/Program.cs", "using StarWarsCopilot.Agents;")]
    [InlineData("8-agents/after/StarWarsCopilot/Program.cs", "tools.Add(new StoryGenerationAgent(chatClient, tools).AsTool());")]
    [InlineData("8-agents/after/StarWarsCopilot/Program.cs", "If you are asked to create a story, use the StoryAgent")]
    [InlineData("8-agents/after/StarWarsCopilot/Agents/StoryAgent.cs", "StarWarsStoryAgent")]
    [InlineData("8-agents/after/StarWarsCopilot/Agents/StorySummaryAgent.cs", "StarWarsStorySummaryAgent")]
    [InlineData("8-agents/after/StarWarsCopilot/Agents/ImageGenerationAgent.cs", "GenerateStarWarsImageTool")]
    [InlineData("8-agents/after/StarWarsCopilot/Agents/StoryGenerationAgent.cs", "StoryAgent")]
    [InlineData("8-agents/after/StarWarsCopilot/Agents/StoryGenerationAgent.cs", "StorySummaryAgent")]
    [InlineData("8-agents/after/StarWarsCopilot/Agents/StoryGenerationAgent.cs", "ImageGenerationAgent")]
    public void Step_artifacts_include_expected_behavioral_markers(string relativePath, string marker)
    {
        var content = ReadFile(relativePath);
        Assert.Contains(marker, content, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("1-chat-with-copilot/after/StarWarsCopilot/Program.cs", false, false, false, false)]
    [InlineData("2-chat-history-and-message-roles/after/StarWarsCopilot/Program.cs", true, false, false, false)]
    [InlineData("3-llm-choice/after/StarWarsCopilot/Program.cs", true, false, false, false)]
    [InlineData("4-call-tools/after/StarWarsCopilot/Program.cs", true, true, false, false)]
    [InlineData("5-mcp/after/StarWarsCopilot/Program.cs", true, true, true, false)]
    [InlineData("7-multimodal/after/StarWarsCopilot/Program.cs", true, true, true, true)]
    [InlineData("8-agents/after/StarWarsCopilot/Program.cs", true, true, true, true)]
    public void Copilot_steps_progressively_add_capabilities(
        string relativePath,
        bool expectsHistory,
        bool expectsToolInvocation,
        bool expectsMcpClient,
        bool expectsRetryInstruction)
    {
        var content = ReadFile(relativePath);

        Assert.Equal(expectsHistory, content.Contains("new List<ChatMessage>", StringComparison.Ordinal));
        Assert.Equal(expectsToolInvocation, content.Contains("UseFunctionInvocation()", StringComparison.Ordinal));
        Assert.Equal(expectsMcpClient, content.Contains("McpClient.CreateAsync", StringComparison.Ordinal));
        Assert.Equal(expectsRetryInstruction, content.Contains("If a tool responds asking you to call it again", StringComparison.Ordinal));
    }

    [Fact]
    public void Mcp_server_steps_progressively_add_tools()
    {
        var step5 = ReadFile("5-mcp/after/StarWarsMCPServer/StarWarsTools.cs");
        var step6 = ReadFile("6-rag/after/StarWarsMCPServer/StarWarsTools.cs");
        var step7 = ReadFile("7-multimodal/after/StarWarsMCPServer/StarWarsTools.cs");

        Assert.Contains("WookiepediaTool", step5, StringComparison.Ordinal);
        Assert.DoesNotContain("StarWarsPurchaseTool", step5, StringComparison.Ordinal);
        Assert.DoesNotContain("GenerateStarWarsImageTool", step5, StringComparison.Ordinal);

        Assert.Contains("WookiepediaTool", step6, StringComparison.Ordinal);
        Assert.Contains("StarWarsPurchaseTool", step6, StringComparison.Ordinal);
        Assert.DoesNotContain("GenerateStarWarsImageTool", step6, StringComparison.Ordinal);

        Assert.Contains("WookiepediaTool", step7, StringComparison.Ordinal);
        Assert.Contains("StarWarsPurchaseTool", step7, StringComparison.Ordinal);
        Assert.Contains("GenerateStarWarsImageTool", step7, StringComparison.Ordinal);
    }

    [Fact]
    public void Step6_rag_data_seed_has_expected_entities()
    {
        var figurinesJson = ReadFile("6-rag/dataloader/figurines.json");
        Assert.Contains("\"Id\":", figurinesJson, StringComparison.Ordinal);
        Assert.Contains("\"Name\":", figurinesJson, StringComparison.Ordinal);
        Assert.Contains("\"Description\":", figurinesJson, StringComparison.Ordinal);
        Assert.Contains("Luke Skywalker", figurinesJson, StringComparison.Ordinal);
        Assert.Contains("Emperor Palpatine", figurinesJson, StringComparison.Ordinal);

        var loaderProgram = ReadFile("6-rag/dataloader/Program.cs");
        Assert.Contains("Ben Smith", loaderProgram, StringComparison.Ordinal);
        Assert.Contains("Anakin Skywalker", loaderProgram, StringComparison.Ordinal);
        Assert.Contains("orderId == 66", loaderProgram, StringComparison.Ordinal);
    }

    [Fact]
    public void Side_by_side_behavior_checks_require_python_steps_when_present()
    {
        var failures = new List<string>();

        foreach (var step in StepRoots)
        {
            var csharpAfter = Path.Combine(RepoRoot, step, "after");
            var pythonAfter = Path.Combine(RepoRoot, step, "after-python");

            if (!Directory.Exists(csharpAfter))
            {
                failures.Add($"{step}: missing C# after folder.");
                continue;
            }

            if (!Directory.Exists(pythonAfter))
            {
                continue;
            }

            CompareStepBehaviors(step, csharpAfter, pythonAfter, failures);
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void Side_by_side_behavior_checks_require_python_port_for_all_steps_when_enabled()
    {
        var requireFullPort = string.Equals(
            Environment.GetEnvironmentVariable("REQUIRE_PYTHON_PARITY"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (!requireFullPort)
        {
            return;
        }

        var missing = StepRoots
            .Where(step => !Directory.Exists(Path.Combine(RepoRoot, step, "after-python")))
            .ToList();

        Assert.True(
            missing.Count == 0,
            $"Missing after-python folders for parity: {string.Join(", ", missing)}");
    }

    private static void CompareStepBehaviors(
        string step,
        string csharpAfter,
        string pythonAfter,
        List<string> failures)
    {
        var csharpProgramFiles = Directory.GetFiles(csharpAfter, "Program.cs", SearchOption.AllDirectories);
        if (csharpProgramFiles.Length == 0)
        {
            failures.Add($"{step}: no C# Program.cs found under after.");
            return;
        }

        var pythonFiles = Directory.GetFiles(pythonAfter, "*.py", SearchOption.AllDirectories);
        var pythonEntryFiles = pythonFiles
            .Where(file =>
            {
                var name = Path.GetFileName(file);
                return name.Equals("program.py", StringComparison.OrdinalIgnoreCase) ||
                       name.Equals("main.py", StringComparison.OrdinalIgnoreCase) ||
                       name.Equals("app.py", StringComparison.OrdinalIgnoreCase) ||
                       name.Equals("server.py", StringComparison.OrdinalIgnoreCase);
            })
            .ToArray();

        if (pythonFiles.Length == 0 || pythonEntryFiles.Length == 0)
        {
            failures.Add($"{step}: after-python exists, but no Python files or entrypoint named program.py/main.py/app.py/server.py found.");
            return;
        }

        var combinedCSharp = string.Join(Environment.NewLine, csharpProgramFiles.Select(File.ReadAllText));
        var combinedPython = string.Join(Environment.NewLine, pythonFiles.Select(File.ReadAllText));

        EnsureParityToken(step, combinedCSharp, combinedPython, "User >", "user");
        EnsureParityToken(step, combinedCSharp, combinedPython, "Assistant >", "assistant");

        if (combinedCSharp.Contains("new List<ChatMessage>", StringComparison.Ordinal))
        {
            EnsureParityToken(step, combinedCSharp, combinedPython, "ChatRole.System", "system");
            EnsureParityToken(step, combinedCSharp, combinedPython, "ChatRole.User", "user");
            EnsureParityToken(step, combinedCSharp, combinedPython, "ChatRole.Assistant", "assistant");
        }

        if (combinedCSharp.Contains("UseFunctionInvocation()", StringComparison.Ordinal))
        {
            EnsureParityToken(step, combinedCSharp, combinedPython, "WookiepediaTool", "wookiepedia");
        }

        if (combinedCSharp.Contains("McpClient.CreateAsync", StringComparison.Ordinal))
        {
            EnsureParityToken(step, combinedCSharp, combinedPython, "McpClient", "mcp");
        }

        if (combinedCSharp.Contains("StoryGenerationAgent", StringComparison.Ordinal))
        {
            EnsureParityToken(step, combinedCSharp, combinedPython, "StoryGenerationAgent", "story");
        }

        foreach (var csFile in Directory.GetFiles(csharpAfter, "*.cs", SearchOption.AllDirectories))
        {
            var csContent = File.ReadAllText(csFile);
            var fileName = Path.GetFileName(csFile);

            if (fileName.Equals("StarWarsTools.cs", StringComparison.Ordinal))
            {
                EnsureIfPresent(step, csContent, combinedPython, "WookiepediaTool", "wookiepedia", failures);
                EnsureIfPresent(step, csContent, combinedPython, "StarWarsPurchaseTool", "purchase", failures);
                EnsureIfPresent(step, csContent, combinedPython, "GenerateStarWarsImageTool", "image", failures);
                EnsureIfPresent(step, csContent, combinedPython, "content_policy_violation", "content_policy", failures);
            }

            if (fileName.EndsWith("Options.cs", StringComparison.Ordinal))
            {
                EnsureIfPresent(step, csContent, combinedPython, "OpenAI:Endpoint", "endpoint", failures);
                EnsureIfPresent(step, csContent, combinedPython, "AIInference:Endpoint", "ai_inference", failures);
                EnsureIfPresent(step, csContent, combinedPython, "Tavily:ApiKey", "tavily", failures);
                EnsureIfPresent(step, csContent, combinedPython, "AzureStorage:ConnectionString", "azure_storage", failures);
                EnsureIfPresent(step, csContent, combinedPython, "ImageGeneration:ModelName", "image_generation", failures);
            }
        }
    }

    private static void EnsureIfPresent(
        string step,
        string csharpContent,
        string pythonContent,
        string csharpToken,
        string pythonToken,
        List<string> failures)
    {
        if (!csharpContent.Contains(csharpToken, StringComparison.Ordinal))
        {
            return;
        }

        if (!ContainsLoose(pythonContent, pythonToken))
        {
            failures.Add($"{step}: expected Python parity marker '{pythonToken}' for C# token '{csharpToken}'.");
        }
    }

    private static void EnsureParityToken(
        string step,
        string csharpContent,
        string pythonContent,
        string csharpToken,
        string pythonToken)
    {
        if (!csharpContent.Contains(csharpToken, StringComparison.Ordinal))
        {
            return;
        }

        Assert.True(
            ContainsLoose(pythonContent, pythonToken),
            $"{step}: expected Python parity marker '{pythonToken}' for C# token '{csharpToken}'.");
    }

    private static bool ContainsLoose(string content, string token)
    {
        return Regex.IsMatch(content, Regex.Escape(token), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static string ReadFile(string relativePath)
    {
        var fullPath = Path.Combine(RepoRoot, relativePath);
        Assert.True(File.Exists(fullPath), $"Expected file not found: {relativePath}");
        return File.ReadAllText(fullPath);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root.");
    }
}
