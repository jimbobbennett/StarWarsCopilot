using Microsoft.Extensions.Configuration;

namespace StarWarsMCPServer;

public static class ToolsOptions
{
    private static readonly string? _tavilyApiKey;
    private static readonly string? _azureStorageConnectionString;

    static ToolsOptions()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var secretProvider = config.Providers.First();
        if (!secretProvider.TryGet("Tavily:ApiKey", out _tavilyApiKey))
        {
            throw new InvalidOperationException("Tavily:ApiKey is not configured in User Secrets.");
        }
        if (!secretProvider.TryGet("AzureStorage:ConnectionString", out _azureStorageConnectionString))
        {
            throw new InvalidOperationException("AzureStorage:ConnectionString is not configured in User Secrets.");
        }
    }

    public static string TavilyApiKey => _tavilyApiKey!;
    public static string? AzureStorageConnectionString => _azureStorageConnectionString;
}