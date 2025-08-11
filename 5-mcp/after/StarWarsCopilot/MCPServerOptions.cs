using System.ComponentModel.DataAnnotations;

namespace StarWarsCopilot;

/// <summary>
/// Configuration settings for MCP Servers
/// </summary>
public class MCPServerOptions
{
    public const string SectionName = "MCPServers";

    /// <summary>
    /// The name of the MCP server
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The command to run the MCP server
    /// </summary>
    [Required]
    public string Command { get; set; } = string.Empty;

    /// <summary>
    /// The arguments to pass to the MCP server command
    /// </summary>
    [Required]
    public List<string> Arguments { get; set; } = [];
}