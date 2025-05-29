// Controllers/EventsController.cs

/*L'endpoint /api/events/sse diffuse en continu les notifications. 
Chaque message est formaté selon le protocole SSE 
(préfixé par data: suivi d'un saut de ligne, puis une ligne vide) 
pour être interprété côté client par un EventSource.*/

using Microsoft.AspNetCore.Mvc;
using MyCrudBackend.Notifications;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MyCrudBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public EventsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("sse")]
        public async Task GetSse(CancellationToken cancellationToken)
        {
            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";

            // Utilisation d'une file locale pour accumuler les messages reçus par l'événement (pour assurer un traitement asynchrone).
            var messages = new ConcurrentQueue<string>();
            var notifyEvent = new SemaphoreSlim(0);

            void NotificationHandler(string message)
            {
                messages.Enqueue(message);
                notifyEvent.Release();
            }

            _notificationService.OnNotification += NotificationHandler;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Attente d'un nouveau message.
                    await notifyEvent.WaitAsync(cancellationToken);

                    // Lit tous les messages en attente.
                    while (messages.TryDequeue(out var msg))
                    {
                        var data = $"data: {msg}\n\n";
                        var bytes = Encoding.UTF8.GetBytes(data);
                        await Response.Body.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                        await Response.Body.FlushAsync(cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // La connexion a été annulée.
            }
            finally
            {
                _notificationService.OnNotification -= NotificationHandler;
            }
        }
    }
}