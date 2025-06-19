using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Areas.Identity.Data;

namespace Backend.Models
{
    public enum AttendanceStatus
    {
        Aanwezig,
        Misschien,
        NietAanwezig
    }

    public class EventAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventID { get; set; }
        public EventsModel Event { get; set; }

        [Required]
        public string UserId { get; set; }
        public BackendUser User { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }
        public string? Comment { get; set; }
    }
}
