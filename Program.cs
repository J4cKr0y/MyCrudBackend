// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyCrudBackend.Data;
using MyCrudBackend.Notifications;
using MyCrudBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajout des contrôleurs
builder.Services.AddControllers();

// Injection de dépendances
builder.Services.AddSingleton<ITodoRepository, TodoRepository>();
builder.Services.AddSingleton<INotificationService, NotificationService>(); // Service de notification partagé
builder.Services.AddSingleton<ITodoService, TodoService>();

var app = builder.Build();

// Configuration du routage
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
