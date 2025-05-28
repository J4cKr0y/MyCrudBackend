// Services/TodoService.cs

/*La logique métier est entièrement encapsulée dans le service TodoService.
Cette couche traite les règles applicatives 
(ex: envoi de notif après création/modification/suppression d'un item).*/

using MyCrudBackend.Data;
using MyCrudBackend.Models;
using MyCrudBackend.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<TodoItem>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<TodoItem?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<TodoItem> CreateAsync(TodoItem item)
        {
            var created = await _repository.CreateAsync(item);
            _notificationService.EnqueueNotification($"[Create] Item créé : {created.Id}");
            return created;
        }

        public async Task UpdateAsync(TodoItem item)
        {
            await _repository.UpdateAsync(item);
            _notificationService.EnqueueNotification($"[Update] Item mis à jour : {item.Id}");
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            _notificationService.EnqueueNotification($"[Delete] Item supprimé : {id}");
        }
    }
}
