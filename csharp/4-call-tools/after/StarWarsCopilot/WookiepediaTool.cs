using System.Text.Json;
using Microsoft.Extensions.AI;

class WookiepediaTool(string apiKey) : AIFunction
{
    private readonly string _apiKey = apiKey;

    public override string Name => "WookiepediaTool";

    private readonly HttpClient _httpClient = new();

    public override string Description =>
        "A tool for getting information on Star Wars from Wookiepedia. This tool takes a prompt as a query and returns a list of results from Wookiepedia.";

    public override JsonElement JsonSchema => JsonDocument.Parse($@"
        {{
            ""title"" : ""{Name}"",
            ""description"": ""{Description}"",
            ""type"": ""object"",
            ""properties"": {{
                ""query"" : {{ 
                    ""type"": ""string"",
                    ""description"": ""The query to search for information on Wookiepedia.""
                }}
            }},
            ""required"": [""query""]
        }}
    ").RootElement;

    public override JsonElement? ReturnJsonSchema => JsonDocument.Parse(@"
        {
            ""title"": ""WookiepediaToolResult"",
            ""type"": ""object"",
            ""properties"": {
                ""query"": { ""type"": ""string"" },
                ""answer"": { ""type"": ""string"" },
                ""results"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""url"": { ""type"": ""string"" },
                            ""title"": { ""type"": ""string"" },
                            ""content"": { ""type"": ""string"" },
                            ""score"": { ""type"": ""number"" },
                            ""raw_content"": { ""type"": [""string"", ""null""] }
                        },
                        ""required"": [""url"", ""title"", ""content"", ""score"", ""raw_content""]
                    }
                }
            },
            ""required"": [""query"", ""answer"", ""images"", ""results""]
        }
    ").RootElement;

    protected async override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var requestBody = new
        {
            query = arguments["query"]!.ToString(),
            include_answer = "advanced",
            include_domains = new[] { "https://starwars.fandom.com/" }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync("https://api.tavily.com/search", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}