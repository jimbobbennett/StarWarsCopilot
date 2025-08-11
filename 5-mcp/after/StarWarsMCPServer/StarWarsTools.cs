using System.ComponentModel;
using System.Text.Json;

using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using StarWarsMCPServer;

[McpServerToolType]
public static class StarWarsTools
{
    private readonly static ToolsOptions _toolsOptions = new();

    private readonly static HttpClient _httpClient = new();

    static StarWarsTools()
    {
        // Build the configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Get the Tools configuration
        _toolsOptions = configuration.GetSection(ToolsOptions.SectionName)
                                     .Get<ToolsOptions>()!;

        if (_toolsOptions == null)
        {
            throw new InvalidOperationException("Tools configuration is missing. Please check your appsettings.json file.");
        }

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_toolsOptions.TavilyApiKey}");
    }

    [McpServerTool(Name = "WookiepediaTool"),
     Description("A tool for getting information on Star Wars from Wookiepedia. " +
                 "This tool takes a prompt as a query and returns a list of results from Wookiepedia.")]
    public static async Task<string> QueryTheWeb([Description("The query to search for information on Wookiepedia.")] string query)
    {
        var requestBody = new
        {
            query,
            include_answer = "advanced",
            include_domains = new[] { "https://starwars.fandom.com/" }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync("https://api.tavily.com/search", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}