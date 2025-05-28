// Services/ITodoService.cs
using MyCrudBackend.Models;
using System.Collections.Generic;

namespace MyCrudBackend.Services
{
    public interface ITodoService
    {
        IEnumerable<TodoItem> GetAll();
        TodoItem? GetById(int id);
        TodoItem Create(TodoItem item);
        void Update(TodoItem item);
        void Delete(int id);
    }
}