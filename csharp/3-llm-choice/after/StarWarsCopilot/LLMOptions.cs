using Microsoft.Extensions.Configuration;

namespace StarWarsCopilot;

public static class LLMOptions
{
    private static readonly string? _endpoint;
    private static readonly string? _apiKey;
    private static readonly string? _model;
    
    private static readonly string? _aiInferenceEndpoint;
    private static readonly string? _aiInferenceModel;

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
        if (!secretProvider.TryGet("AIInference:Endpoint", out _aiInferenceEndpoint))
        {
            throw new InvalidOperationException("AIInference:Endpoint is not configured in User Secrets.");
        }
        if (!secretProvider.TryGet("AIInference:ModelName", out _aiInferenceModel))
        {
            throw new InvalidOperationException("AIInference:ModelName is not configured in User Secrets.");
        }
    }

    public static string Endpoint => _endpoint!;
    public static string ApiKey => _apiKey!;
    public static string Model => _model!;
    public static string AIInferenceEndpoint => _aiInferenceEndpoint!;
    public static string AIInferenceModel => _aiInferenceModel!;

}