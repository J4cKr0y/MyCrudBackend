// Models/TodoItem.cs
namespace MyCrudBackend.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
