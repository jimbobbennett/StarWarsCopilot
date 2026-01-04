using System.ClientModel;
using System.ComponentModel;
using System.Text.Json;

using Azure.AI.OpenAI;
using Azure.Data.Tables;
using OpenAI.Images;

using ModelContextProtocol.Server;
using OpenAI;

namespace StarWarsMCPServer;

[McpServerToolType]
public static class StarWarsTools
{
    private readonly static HttpClient _httpClient = new();

    static StarWarsTools()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ToolsOptions.TavilyApiKey}");
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


    private static async Task<List<TableEntity>> GetOrders(TableServiceClient serviceClient, int orderNumber, string customerName)
    {
        var ordersFilter = new List<string>();

        if (orderNumber > 0)
            ordersFilter.Add($"RowKey eq '{orderNumber}'");

        if (!string.IsNullOrWhiteSpace(customerName))
            ordersFilter.Add($"CustomerName eq '{customerName.Trim()}'");

        var combinedOrderFilter = ordersFilter.Count == 0 ? null : string.Join(" and ", ordersFilter);

        var ordersTbl = serviceClient.GetTableClient("Orders");
        var ordersQuery = ordersTbl.QueryAsync<TableEntity>(combinedOrderFilter);
        var orders = new List<TableEntity>();
        await foreach (var order in ordersQuery)
        {
            orders.Add(order);
        }

        return orders;
    }

    private static async Task<Dictionary<string, TableEntity>> GetFigurines(TableServiceClient serviceClient, string characterName)
    {
        var figurinesFilter = new List<string>();

        if (!string.IsNullOrWhiteSpace(characterName))
            figurinesFilter.Add($"Name eq '{characterName.Trim()}'");

        var combinedFigurineFilter = figurinesFilter.Count == 0 ? null : string.Join(" and ", figurinesFilter);

        var figurinesTbl = serviceClient.GetTableClient("Figurines");
        var figurinesQuery = figurinesTbl.QueryAsync<TableEntity>(combinedFigurineFilter);
        var figurines = new List<TableEntity>();
        await foreach (var figurine in figurinesQuery)
        {
            figurines.Add(figurine);
        }

        return figurines.ToDictionary(f => f.RowKey);
    }

    [McpServerTool(Name = "StarWarsPurchaseTool"),
     Description("A tool for getting information on Star Wars figurine purchases." +
                 "This tool can take either an order number, character name, and customer name as parameters, and returns a list of purchases." +
                 "Only one of the parameters is required, but more can be used to narrow down the results.")]
    public static async Task<string> GetStarWarsPurchases([Description("The order number")] int orderNumber = -1,
                                                          [Description("The name of the figurine or character ordered")] string characterName = "",
                                                          [Description("The name of the customer who ordered the figurines")] string customerName = "")
    {
        try
        {
            if (orderNumber <= 0 &&
                string.IsNullOrWhiteSpace(characterName) &&
                string.IsNullOrWhiteSpace(customerName))
            {
                return JsonSerializer.Serialize(new { error = "At least one parameter is required: orderNumber, characterName, or customerName." });
            }

            var serviceClient = new TableServiceClient(ToolsOptions.AzureStorageConnectionString);

            // Get the orders that match the provided order number or customer name
            var orders = await GetOrders(serviceClient, orderNumber, customerName);

            // Get the figurines that match the character name
            // If this parameter is not provided, it will return all figurines
            // Otherwise there should be only one
            var figurines = await GetFigurines(serviceClient, characterName);
            if (figurines.Count == 0 && !string.IsNullOrWhiteSpace(characterName))
            {
                return JsonSerializer.Serialize(new { error = $"No figurines found for character '{characterName}'." });
            }

            var results = new List<object>();

            var orderFigTbl = serviceClient.GetTableClient("OrderFigurines");
            foreach (var order in orders)
            {
                var orderId = order.RowKey;
                var figuresFilter = $"PartitionKey eq '{orderId}'";
                // If we are filtering by character name, we need to check if the figurine matches
                if (!string.IsNullOrWhiteSpace(characterName))
                {
                    figuresFilter += $" and FigurineName eq '{characterName}'";
                }

                var orderFigurines = orderFigTbl.QueryAsync<TableEntity>(figuresFilter);

                // Get all the figurines for this order that match the character name
                // If character name is not provided, it will return all figurines for the order
                var figurinesList = new List<object>();
                await foreach (var f in orderFigurines)
                {
                    if (figurines.TryGetValue(f.RowKey, out var figurine))
                    {
                        figurinesList.Add(new
                        {
                            FigurineId = f.RowKey,
                            FigurineName = figurine.GetString("Name"),
                            Price = figurine.GetDouble("Price"),
                            Description = figurine.GetString("Description")
                        });
                    }
                }

                // Only add the order to the results if it has figurines
                if (figurinesList.Count != 0)
                {
                    results.Add(new
                    {
                        OrderId = orderId,
                        CustomerId = order.GetString("CustomerID"),
                        CustomerName = order.GetString("CustomerName"),
                        TotalCost = order.GetDouble("TotalCost"),
                        Figures = figurinesList
                    });
                }
            }

            // Return the results as a JSON string
            return JsonSerializer.Serialize(results);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { error = ex.Message });
        }
    }

    [McpServerTool(Name = "GenerateStarWarsImageTool"),
     Description("A tool for generating images based on Star Wars. This tool takes a description" +
                 "of the required image and returns a URL to the generated image.")]
    public static async Task<string> GenerateStarWarsImage([Description("The description of the Star Wars image to generate.")] string description)
    {
        try
        {
            // Validate the description
            if (string.IsNullOrWhiteSpace(description))
            {
                return JsonSerializer.Serialize(new { error = "Description cannot be empty." });
            }

            var client = new AzureOpenAIClient(new Uri(ToolsOptions.ImageGenerationEndpoint),
                                               new Azure.AzureKeyCredential(ToolsOptions.ImageGenerationApiKey))
                                            .GetImageClient(ToolsOptions.ImageGenerationModel);

            // Generate the image
            var generatedImage = await client.GenerateImageAsync($"""
                Generate a cartoon style image based on the following description or story:
                "{description}"

                The image should be in the style of a parody of the original Star Wars trilogy, looking like a movie from the 1970s or 1980s.
                Make the image high quality, hyper real, with vivid colors and a cinematic feel from an animated movie.

                This image is designed to be the used on front cover of a book that matches the given description or story.
                """,
                    new ImageGenerationOptions { Size = GeneratedImageSize.W1024xH1024 });

            // Return the URL of the generated image
            return JsonSerializer.Serialize(new { imageUrl = generatedImage.Value.ImageUri });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("content_policy_violation"))
            {
                return JsonSerializer.Serialize(new
                {
                    error = @"
                    A content error occurred while generating the image.
                    Please retry this tool with an adjusted prompt, such as changing named characters to very detailed
                    descriptions of the characters. Include details like race, gender, age, dress style, distinguishing features
                    (e.g., 'an old, small, green Jedi Master with pointy ears, a tuft of white hair and wrinkles' instead of 'Yoda').
                    If the description contains anything sexual or violent, replace with a more PG version of the description.
                    "
                });
            }
            
            return JsonSerializer.Serialize(new { error = ex.Message });
        }
    }
}