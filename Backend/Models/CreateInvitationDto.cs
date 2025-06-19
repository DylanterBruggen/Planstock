namespace Backend.Models
{
    public class CreateInvitationDto
    {
        public int EventId { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
    }
}
