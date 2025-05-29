// Notifications/NotificationService.cs
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MyCrudBackend.Notifications
{
    public class NotificationService : INotificationService, IDisposable
    {
        public event Action<string>? OnNotification;
        private readonly ConcurrentQueue<string> _queue = new(); //garantit l'ordre d'arrivée des messages
        private readonly CancellationTokenSource _cts = new();

        public NotificationService()
        {
            // Démarrage d'un traitement asynchrone en continu pour vider la file d'attente en FIFO.
            Task.Run(async () => await ProcessQueue());
        }

        public void EnqueueNotification(string message)
        {
            _queue.Enqueue(message);
        }

        private async Task ProcessQueue()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out string? message) && message != null)
                {
                    // Notification immédiate à tous les abonnés.
                    OnNotification?.Invoke(message);
                }
                else
                {
                    await Task.Delay(100, _cts.Token);
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}