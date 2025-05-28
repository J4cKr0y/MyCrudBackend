// Services/TodoService.cs

/*La logique métier est entièrement encapsulée dans le service TodoService.
Cette couche traite les règles applicatives 
(ex: envoi de notif après création/modification/suppression d'un item).*/

using MyCrudBackend.Data;
using MyCrudBackend.Models;
using MyCrudBackend.Notifications;
using System.Collections.Generic;

namespace MyCrudBackend.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;
        private readonly INotificationService _notificationService;

        public TodoService(ITodoRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public IEnumerable<TodoItem> GetAll() => _repository.GetAll();

        public TodoItem? GetById(int id) => _repository.GetById(id);

        public TodoItem Create(TodoItem item)
        {
            // Todo : Ajouter des règles de validation
            var created = _repository.Create(item);
            _notificationService.EnqueueNotification($"[Create] Item créé : {created.Id}");
            return created;
        }

        public void Update(TodoItem item)
        {
            _repository.Update(item);
            _notificationService.EnqueueNotification($"[Update] Item mis à jour : {item.Id}");
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
            _notificationService.EnqueueNotification($"[Delete] Item supprimé : {id}");
        }
    }
}