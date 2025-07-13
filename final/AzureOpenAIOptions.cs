using System.ComponentModel.DataAnnotations;

namespace StarWarsCopilot;

/// <summary>
/// Configuration settings for Azure OpenAI service
/// </summary>
public class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    /// <summary>
    /// The model ID to use for chat completion
    /// </summary>
    [Required]
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// The Azure OpenAI endpoint URL
    /// </summary>
    [Required]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// The API key for authentication
    /// Note: In production, consider using Managed Identity instead
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;
}
