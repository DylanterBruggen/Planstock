using Backend.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("EvenementenNieuw")]
    public class EventsModel
    {
        [Key]
        public int EventID { get; set; }

        public string? EventName { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? MaxGuests { get; set; }
        public string? CoverPhoto { get; set; }
        public string? UserId { get; set; }

        public BackendUser? User { get; set; }

        public int? LocationID { get; set; }
        public int? PrivacyID { get; set; }
        public int? CategoryID { get; set; }
    }
}
