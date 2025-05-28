// Data/ITodoRepository.cs
using MyCrudBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCrudBackend.Data
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<TodoItem?> GetByIdAsync(int id);
        Task<TodoItem> CreateAsync(TodoItem item);
        Task UpdateAsync(TodoItem item);
        Task DeleteAsync(int id);
    }
}
