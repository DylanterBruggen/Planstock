using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{


    // API controller for managing events
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly BackendDbContext _context;

        // Constructor that injects the database context
        public EventsController(BackendDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        // Returns all events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventsModel>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/Events/{id}
        // Returns a specific event by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<EventsModel>> GetEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
                return NotFound(); // 404 if not found

            return Ok(evt);
        }

        // POST: api/Events
        // Creates a new event
        [HttpPost]
        public async Task<ActionResult<EventsModel>> CreateEvent(EventsModel evt)
        {
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            // Returns 201 with location header
            return CreatedAtAction(nameof(GetEvent), new { id = evt.EventID }, evt);
        }

        // PUT: api/Events/{id}
        // Updates an existing event
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventsModel evt)
        {
            if (id != evt.EventID)
                return BadRequest("ID in URL does not match EventID in body.");

            _context.Entry(evt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Events.Any(e => e.EventID == id))
                    return NotFound(); // 404 if not found

                throw; // Unknown issue
            }

            return NoContent(); // 204 Success
        }

        // DELETE: api/Events/{id}
        // Deletes an event
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
                return NotFound();

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class AttendanceRequest
        {
            public AttendanceStatus Status { get; set; }
            public string? Comment { get; set; }
        }

        [HttpPost("{eventId}/attendance")]
        [Authorize]
        public async Task<IActionResult> SetAttendance(int eventId, [FromBody] AttendanceRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var existing = await _context.EventAttendances
                .FirstOrDefaultAsync(a => a.EventID == eventId && a.UserId == userId);

            if (existing == null)
            {
                existing = new EventAttendance
                {
                    EventID = eventId,
                    UserId = userId,
                    Status = request.Status,
                    Comment = request.Comment
                };
                _context.EventAttendances.Add(existing);
            }
            else
            {
                existing.Status = request.Status;
                existing.Comment = request.Comment;
                _context.EventAttendances.Update(existing);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Aanwezigheid bevestigd." });
        }

    }
}
