using Azure.Data.Tables;
using System.Text.Json;

// Load figurines from JSON
var figurinesJson = await File.ReadAllTextAsync("figurines.json");
var figurines = JsonSerializer.Deserialize<List<Figurine>>(figurinesJson);

var connStr = "<connection_string>"; // Replace with your Azure Table Storage connection string
var serviceClient = new TableServiceClient(connStr);

var figurinesTbl = serviceClient.GetTableClient("Figurines");
var ordersTbl = serviceClient.GetTableClient("Orders");
var orderFigTbl = serviceClient.GetTableClient("OrderFigurines");

// Insert Figurines
foreach (var figurine in figurines!)
{
    figurinesTbl.UpsertEntity(new TableEntity("Figurines", figurine.Id)
    {
        {"Name", figurine.Name},
        {"Description", figurine.Description},
        {"Price", figurine.Price}
    });
}

var customers = new List<(string Id, string Name)>
{
    ("C001", "Luke Johnson"),
    ("C002", "Leia Parker"),
    ("C003", "Han Richards"),
    ("C004", "Ben Smith"),
    ("C005", "Yoda Masterson"),
    ("C006", "Rey Fisher"),
    ("C007", "Anakin Skywalker"),
    ("C008", "Padmé Amidala"),
    ("C009", "Lando Calrissian"),
    ("C010", "Obi Wan")
};

var rnd = new Random();

for (int i = 0; i < 10; i++)
{
    int orderId = 60 + i;
    var (customerId, customerName) = customers[i];

    List<Figurine> chosenFigurines;

    if (orderId == 66)
    {
        chosenFigurines = [.. figurines.Where(f => f.Id == "F019")];
    }
    else
    {
        chosenFigurines = [.. figurines.OrderBy(x => rnd.Next()).Take(rnd.Next(1, 5))];
    }

    double totalCost = chosenFigurines.Sum(f => f.Price);

    ordersTbl.UpsertEntity(new TableEntity("Orders", orderId.ToString())
    {
        {"CustomerID", customerId},
        {"CustomerName", customerName},
        {"TotalCost", totalCost}
    });

    foreach (var figurine in chosenFigurines)
    {
        orderFigTbl.UpsertEntity(new TableEntity(orderId.ToString(), figurine.Id));
    }
}

Console.WriteLine("Data insertion complete.");


// Define Figurine class
record Figurine(string Id, string Name, string Description, double Price);