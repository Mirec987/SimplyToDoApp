using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ToDoApp.Data;
using ToDoApp.Hubs;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly ToDoContext _context;
        private readonly IDistributedCache _cache;
        private readonly IHubContext<NotificationHub> _hubContext;
        private const string ToDoCacheKey = "ToDoItems";

        public ToDoItemsController(ToDoContext context, IDistributedCache cache, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _cache = cache;
            _hubContext = hubContext;
        }

        // GET: api/ToDoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoItem>>> GetToDoItems()
        {
            var cachedToDoItems = await _cache.GetStringAsync(ToDoCacheKey);

            if (!string.IsNullOrEmpty(cachedToDoItems))
            {
                var toDoItemsFromCache = JsonSerializer.Deserialize<List<ToDoItem>>(cachedToDoItems);
                return toDoItemsFromCache;
            }

            var toDoItemsFromDb = await _context.ToDoItems.ToListAsync();
            var serializedToDoItems = JsonSerializer.Serialize(toDoItemsFromDb);
            await _cache.SetStringAsync(ToDoCacheKey, serializedToDoItems, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return toDoItemsFromDb;
        }

        // POST: api/ToDoItems
        [HttpPost]
        public async Task<ActionResult<ToDoItem>> PostToDoItem(ToDoItem item)
        {
            _context.ToDoItems.Add(item);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync(ToDoCacheKey);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "New task added", item);

            return CreatedAtAction(nameof(GetToDoItems), new { id = item.Id }, item);
        }

        // PUT: api/ToDoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoItem(int id, ToDoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync(ToDoCacheKey);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Task updated", item);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ToDoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            var item = await _context.ToDoItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.ToDoItems.Remove(item);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync(ToDoCacheKey);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Task deleted", id);

            return NoContent();
        }

        private bool ToDoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}
