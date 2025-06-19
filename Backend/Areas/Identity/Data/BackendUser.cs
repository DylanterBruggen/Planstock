using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Backend.Models;

namespace Backend.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BackendUser class
public class BackendUser : IdentityUser
{
    public ICollection<EventsModel> Events { get; set; } = new List<EventsModel>();
}



