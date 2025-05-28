// Controllers/TodoController.cs
using Microsoft.AspNetCore.Mvc;
using MyCrudBackend.Models;
using MyCrudBackend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCrudBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _service;

        public TodoController(ITodoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll() 
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> Create([FromBody] TodoItem todo)
        {
            var created = await _service.CreateAsync(todo);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TodoItem todo)
        {
            if (id != todo.Id) return BadRequest();
            await _service.UpdateAsync(todo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
