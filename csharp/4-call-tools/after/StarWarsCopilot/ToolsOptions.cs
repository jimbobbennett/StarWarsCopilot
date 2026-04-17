using Microsoft.Extensions.Configuration;

namespace StarWarsCopilot;

public static class ToolsOptions
{
    private static readonly string? _tavilyApiKey;

    static ToolsOptions()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var secretProvider = config.Providers.First();
        if (!secretProvider.TryGet("Tavily:ApiKey", out _tavilyApiKey))
        {
            throw new InvalidOperationException("Tavily:ApiKey is not configured in User Secrets.");
        }
    }

    public static string TavilyApiKey => _tavilyApiKey!;
}