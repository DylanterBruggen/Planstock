using Backend.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class EventInvitation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public EventsModel? Event { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public BackendUser? User { get; set; }

        public string? Email { get; set; }

        public string InviteToken { get; set; } = Guid.NewGuid().ToString();

        public DateTime InvitedAt { get; set; } = DateTime.UtcNow;

        public bool? IsAccepted { get; set; }
    }
}
