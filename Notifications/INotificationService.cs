// Notifications/INotificationService.cs
using System;

namespace MyCrudBackend.Notifications
{
    public interface INotificationService
    {
        event Action<string>? OnNotification;
        void EnqueueNotification(string message);
    }
}