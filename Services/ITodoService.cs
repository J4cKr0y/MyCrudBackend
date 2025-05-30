// Services/ITodoService.cs
using MyCrudBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCrudBackend.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<TodoItem?> GetByIdAsync(int id);
        Task<TodoItem> CreateAsync(TodoItem item);
        Task UpdateAsync(TodoItem item);
        Task DeleteAsync(int id);
    }
}