// Program.cs
using Microsoft.Azure.Cosmos;
using MyCrudBackend.Data;
using MyCrudBackend.Notifications;
using MyCrudBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajout des contrôleurs
builder.Services.AddControllers();

// Configuration Swagger pour le développement
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Récupération des paramètres Cosmos dans la configuration
string? cosmosEndpoint = builder.Configuration["Cosmos:Endpoint"];
string? cosmosPrimaryKey = builder.Configuration["Cosmos:PrimaryKey"];
string databaseId = builder.Configuration["Cosmos:DatabaseId"] ?? "TodoDatabase";
string containerId = builder.Configuration["Cosmos:ContainerId"] ?? "TodoContainer";

// Choix du repository selon la configuration
if (string.IsNullOrEmpty(cosmosEndpoint) || string.IsNullOrEmpty(cosmosPrimaryKey))
{
    Console.WriteLine("⚠️  Configuration Cosmos DB manquante, utilisation du repository en mémoire.");
    builder.Services.AddSingleton<ITodoRepository, TodoRepository>();
}
else
{
    Console.WriteLine("✅ Configuration Cosmos DB détectée.");
    
    // Injection du CosmosClient
    builder.Services.AddSingleton(s => new CosmosClient(cosmosEndpoint, cosmosPrimaryKey));

    // Utilisation de l'implémentation Azure Cosmos pour ITodoRepository
    builder.Services.AddSingleton<ITodoRepository>(s =>
    {
        var cosmosClient = s.GetRequiredService<CosmosClient>();
        return new CosmosTodoRepository(cosmosClient, databaseId, containerId);
    });
}

// Injection des autres services
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Configuration CORS pour les SSE (optionnel)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Configuration du routage (version moderne)
app.MapControllers();

// Initialisation optionnelle de Cosmos DB
if (!string.IsNullOrEmpty(cosmosEndpoint) && !string.IsNullOrEmpty(cosmosPrimaryKey))
{
    await InitializeCosmosDb(app.Services, databaseId, containerId);
}

app.Run();

// Méthode d'initialisation Cosmos DB
async Task InitializeCosmosDb(IServiceProvider services, string databaseId, string containerId)
{
    using var scope = services.CreateScope();
    var cosmosClient = scope.ServiceProvider.GetRequiredService<CosmosClient>();
    
    try
    {
        // Création de la base de données si elle n'existe pas
        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        
        // Création du conteneur si il n'existe pas
        await databaseResponse.Database.CreateContainerIfNotExistsAsync(
            containerId, 
            "/id", // Clé de partition
            400);  // RU/s minimum
            
        Console.WriteLine("✅ Base de données Cosmos DB initialisée avec succès.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erreur lors de l'initialisation Cosmos DB : {ex.Message}");
        Console.WriteLine("L'application continue avec la configuration actuelle.");
    }
}