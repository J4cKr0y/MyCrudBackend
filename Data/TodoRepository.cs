// Data/TodoRepository.cs
using MyCrudBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCrudBackend.Data
{
    public class TodoRepository : ITodoRepository
    {
        private readonly List<TodoItem> _items = new();
        private int _nextId = 1;

        public Task<IEnumerable<TodoItem>> GetAllAsync() 
            => Task.FromResult<IEnumerable<TodoItem>>(_items);

        public Task<TodoItem?> GetByIdAsync(int id) 
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<TodoItem> CreateAsync(TodoItem item)
        {
            item.Id = _nextId++;
            _items.Add(item);
            return Task.FromResult(item);
        }

        public Task UpdateAsync(TodoItem item)
        {
            var existing = _items.FirstOrDefault(x => x.Id == item.Id);
            if (existing is not null)
            {
                existing.Title = item.Title;
                existing.Description = item.Description;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item is not null)
            {
                _items.Remove(item);
            }
            return Task.CompletedTask;
        }
    }
}