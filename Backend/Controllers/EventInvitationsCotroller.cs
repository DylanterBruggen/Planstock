using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventInvitationsController : ControllerBase
    {
        private readonly BackendDbContext _context;

        public EventInvitationsController(BackendDbContext context)
        {
            _context = context;
        }

        // POST: api/EventInvitations
        [HttpPost]
        public async Task<IActionResult> Invite(CreateInvitationDto dto)
        {
            if (!_context.Events.Any(e => e.EventID == dto.EventId))
                return NotFound("Event not found");

            var invitation = new EventInvitation
            {
                EventId = dto.EventId,
                Email = dto.Email,
                UserId = dto.UserId,
                InviteToken = Guid.NewGuid().ToString(),
                InvitedAt = DateTime.UtcNow,
                IsAccepted = null
            };

            _context.EventInvitations.Add(invitation);
            await _context.SaveChangesAsync();

            return Ok(invitation);
        }


        // GET: api/EventInvitations/event/5
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<EventInvitation>>> GetInvitationsByEvent(int eventId)
        {
            var invites = await _context.EventInvitations
                .Where(i => i.EventId == eventId)
                .ToListAsync();

            return Ok(invites);
        }
    }
}
