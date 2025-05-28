// Data/TodoRepository.cs
using MyCrudBackend.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyCrudBackend.Data
{
    public class TodoRepository : ITodoRepository
    {
        private readonly List<TodoItem> _items = new();
        private int _nextId = 1;

        public IEnumerable<TodoItem> GetAll() => _items;

        public TodoItem? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

        public TodoItem Create(TodoItem item)
        {
            item.Id = _nextId++;
            _items.Add(item);
            return item;
        }

        public void Update(TodoItem item)
        {
            var existing = GetById(item.Id);
            if (existing is not null)
            {
                existing.Title = item.Title;
                existing.Description = item.Description;
            }
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item is not null)
            {
                _items.Remove(item);
            }
        }
    }
}