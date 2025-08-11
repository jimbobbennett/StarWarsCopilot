using System.ComponentModel.DataAnnotations;
    
namespace StarWarsAgentCopilot;

/// <summary>
/// Configuration settings for the LLM
/// </summary>
public class LLMOptions
{
    public const string SectionName = "LLM";

    /// <summary>
    /// The model ID to use for chat completion
    /// </summary>
    [Required]
    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// The OpenAI API endpoint URL
    /// </summary>
    [Required]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// The API key for authentication
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;
}