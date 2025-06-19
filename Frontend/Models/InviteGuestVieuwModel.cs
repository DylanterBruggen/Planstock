using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class InviteGuestViewModel
    {
        public int EventId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
