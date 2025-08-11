using System.ComponentModel.DataAnnotations;

namespace StarWarsCopilot;

/// <summary>
/// Configuration settings for the tools
/// </summary>
public class ToolsOptions
{
    public const string SectionName = "Tools";

    /// <summary>
    /// The API key for Tavily
    /// </summary>
    [Required]
    public string TavilyApiKey { get; set; } = string.Empty;
}