using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Data.Entities;

namespace Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>(entity =>
        {
            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(p => p.Description)
                  .HasMaxLength(500);

            entity.Property(p => p.CreatedAt)
                  .IsRequired();

            entity.Property(p => p.LastUpdatedAt)
                  .IsRequired();
        });

        builder.Entity<Todo>(entity =>
        {
            entity.Property(t => t.Title)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(t => t.Description)
                  .HasMaxLength(1000);

            entity.Property(t => t.IsCompleted)
                  .IsRequired();

            entity.Property(t => t.Priority)
                  .IsRequired();

            entity.Property(t => t.CreatedAt)
                  .IsRequired();

            entity.Property(t => t.LastUpdatedAt)
                  .IsRequired();
        });

        builder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(pm => new { pm.ProjectId, pm.UserId });

            entity.Property(pm => pm.Role)
                  .IsRequired()
                  .HasMaxLength(50);
        });
    }
}