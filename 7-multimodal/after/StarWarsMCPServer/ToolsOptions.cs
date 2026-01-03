using Microsoft.Extensions.Configuration;

namespace StarWarsMCPServer;

public static class ToolsOptions
{
    private static readonly string? _tavilyApiKey;
    private static readonly string? _azureStorageConnectionString;
    private static readonly string? _imageGenerationEndpoint;
    private static readonly string? _imageGenerationApiKey;
    private static readonly string? _imageGenerationModel;

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
        if (!secretProvider.TryGet("ImageGeneration:Endpoint", out _imageGenerationEndpoint))
        {
            throw new InvalidOperationException("ImageGeneration:Endpoint is not configured in User Secrets.");
        }
        if (!secretProvider.TryGet("ImageGeneration:ApiKey", out _imageGenerationApiKey))
        {
            throw new InvalidOperationException("ImageGeneration:ApiKey is not configured in User Secrets.");
        }
        if (!secretProvider.TryGet("ImageGeneration:ModelName", out _imageGenerationModel))
        {
            throw new InvalidOperationException("ImageGeneration:ModelName is not configured in User Secrets.");
        }
    }

    public static string TavilyApiKey => _tavilyApiKey!;
    public static string AzureStorageConnectionString => _azureStorageConnectionString!;
    public static string ImageGenerationEndpoint => _imageGenerationEndpoint!;
    public static string ImageGenerationApiKey => _imageGenerationApiKey!;
    public static string ImageGenerationModel => _imageGenerationModel!;
}