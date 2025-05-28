// Controllers/TodoController.cs
using Microsoft.AspNetCore.Mvc;
using MyCrudBackend.Models;
using MyCrudBackend.Services;
using System.Collections.Generic;

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
        public ActionResult<IEnumerable<TodoItem>> GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public ActionResult<TodoItem> Get(int id)
        {
            var item = _service.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public ActionResult<TodoItem> Create([FromBody] TodoItem todo)
        {
            var created = _service.Create(todo);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TodoItem todo)
        {
            if (id != todo.Id) return BadRequest();
            _service.Update(todo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
