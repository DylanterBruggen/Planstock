using Backend.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class BackendDbContext : IdentityDbContext<BackendUser>
{
    public DbSet<EventInvitation> EventInvitations { get; set; }

    public DbSet<EventsModel> Events { get; set; }
    public DbSet<EventAttendance> EventAttendances { get; set; }

    public BackendDbContext(DbContextOptions<BackendDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        // For user relation
        builder.Entity<EventsModel>()
            .HasOne(e => e.User)
            .WithMany(u => u.Events)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
