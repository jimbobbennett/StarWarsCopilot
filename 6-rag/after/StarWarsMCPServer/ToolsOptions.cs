using System.ComponentModel.DataAnnotations;

namespace StarWarsMCPServer;

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
    
    /// <summary>
    /// The connection string for Azure Storage
    /// </summary>
    [Required]
    public string StorageConnectionString { get; set; } = string.Empty;
}