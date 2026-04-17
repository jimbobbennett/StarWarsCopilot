using Microsoft.Extensions.Configuration;

namespace StarWarsCopilot;

public static class LLMOptions
{
    private static readonly string? _endpoint;
    private static readonly string? _apiKey;
    private static readonly string? _model;

    static LLMOptions()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var secretProvider = config.Providers.First();
        if (!secretProvider.TryGet("OpenAI:Endpoint", out _endpoint))
        {
            throw new InvalidOperationException("OpenAI:Endpoint is not configured in User Secrets.");
        }
        if (!secretProvider.TryGet("OpenAI:ApiKey", out _apiKey))
        {
            throw new InvalidOperationException("OpenAI:ApiKey is not configured in User Secrets.");
        }
        if (!secretProvider.TryGet("OpenAI:ModelName", out _model))
        {
            throw new InvalidOperationException("OpenAI:ModelName is not configured in User Secrets.");
        }
    }

    public static string Endpoint => _endpoint!;
    public static string ApiKey => _apiKey!;
    public static string Model => _model!;

}