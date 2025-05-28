// Program.cs
using Microsoft.Azure.Cosmos;
using MyCrudBackend.Data;
using MyCrudBackend.Notifications;
using MyCrudBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajout des contrôleurs
builder.Services.AddControllers();

// Récupération des paramètres Cosmos dans la configuration
string cosmosEndpoint = builder.Configuration["Cosmos:Endpoint"] 
    ?? throw new Exception("Cosmos Endpoint non configuré dans appsettings.json.");
string cosmosPrimaryKey = builder.Configuration["Cosmos:PrimaryKey"] 
    ?? throw new Exception("Cosmos PrimaryKey non configuré dans appsettings.json.");
string databaseId = builder.Configuration["Cosmos:DatabaseId"] ?? "TodoDatabase";
string containerId = builder.Configuration["Cosmos:ContainerId"] ?? "TodoContainer";

// Injection du CosmosClient
builder.Services.AddSingleton(s => new CosmosClient(cosmosEndpoint, cosmosPrimaryKey));

// Utilisation de l’implémentation Azure Cosmos pour ITodoRepository
builder.Services.AddSingleton<ITodoRepository>(s =>
{
    var cosmosClient = s.GetRequiredService<CosmosClient>();
    return new CosmosTodoRepository(cosmosClient, databaseId, containerId);
});

// Injection des autres services
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<ITodoService, TodoService>();

var app = builder.Build();

// Configuration du routage
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
