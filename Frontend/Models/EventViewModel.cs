using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
	public class EventViewModel
	{
		
			[Key]
			public int EventID { get; set; }

			public string EventName { get; set; }
			public string? Description { get; set; }
			public DateTime Date { get; set; }
			public TimeSpan StartTime { get; set; }
			public TimeSpan EndTime { get; set; }
			public int LocationID { get; set; }
			public int? MaxGuests { get; set; }
			public string? CoverPhoto { get; set; }
		}
	}

