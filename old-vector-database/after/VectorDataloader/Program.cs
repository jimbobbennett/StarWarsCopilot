using Pinecone;

var indexName = "movie-scripts";
var pinecone = new PineconeClient("API_KEY");

// Create the index if it does not exist
var createIndexRequest = new CreateIndexForModelRequest
{
    Name = indexName,
    Cloud = CreateIndexForModelRequestCloud.Aws,
    Region = "us-east-1",
    Embed = new CreateIndexForModelRequestEmbed
    {
        Model = "llama-text-embed-v2",
        FieldMap = new Dictionary<string, object?>()
        {
            { "text", "chunk_text" }
        }
    }
};

var index = await pinecone.CreateIndexForModelAsync(createIndexRequest);

while (!index.Status.Ready)
{
    await Task.Delay(5000); // Wait for the index to be ready
    index = await pinecone.DescribeIndexAsync(indexName);
}

// Get a client for the new index
var indexClient = pinecone.Index(indexName);
var recordNumber = 0;

// Load the scripts into the index
var scriptsPath = Path.Combine(AppContext.BaseDirectory, "movie-scripts");
foreach (var scriptFile in Directory.GetFiles(scriptsPath, "*.md"))
{
    var movieName = Path.GetFileNameWithoutExtension(scriptFile);
    Console.WriteLine($"Processing script: {movieName}");

    List<UpsertRecord> records = [];

    // Chunk the script content into manageable pieces
    var scriptContent = await File.ReadAllTextAsync(scriptFile);
    var chunks = scriptContent.Split(["\n\n"], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    foreach (var chunk in chunks)
    {
        records.Add(new UpsertRecord
        {
            Id = $"rec{recordNumber++}",
            AdditionalProperties =
            {
                ["chunk_text"] = chunk,
                ["movie_name"] = movieName,
            },
        });

        // Pinecone has a limit of 96 records per upsert operation
        if (records.Count >= 96)
        {
            // Upsert the records to Pinecone
            await indexClient.UpsertRecordsAsync("Star Wars", records);
            records.Clear();
        }
    }

    // Upsert any remaining records
    await indexClient.UpsertRecordsAsync("Star Wars", records);
    Console.WriteLine($"\nFinished processing script: {movieName}");
}
