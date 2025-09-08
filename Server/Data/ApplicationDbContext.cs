using Microsoft.EntityFrameworkCore;
using Server.Data.Entities;

namespace Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Token> Tokens { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<TodoProject> Projects { get; set; }
    public DbSet<TodoItem> Items { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- User ---
        builder.Entity<User>(entity =>
        {
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Username).HasMaxLength(25);
            entity.Property(p => p.Email).HasMaxLength(128);
        });

        // --- Token ---
        builder.Entity<Token>(entity =>
        {
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Purpose).HasMaxLength(128);
        });

        // --- TodoProject ---
        builder.Entity<TodoProject>(entity =>
        {
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Name).HasMaxLength(128);
            entity.Property(p => p.Description).HasMaxLength(512);

            entity.HasOne(p => p.Owner)
                  .WithMany()
                  .HasForeignKey(p => p.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict); // prevent cascade loop
        });

        // --- TodoItem ---
        builder.Entity<TodoItem>(entity =>
        {
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Title).HasMaxLength(128);
            entity.Property(p => p.Description).HasMaxLength(512);

            entity.HasOne(i => i.Project)
                  .WithMany(p => p.Items)
                  .HasForeignKey(i => i.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.AssignedTo)
                  .WithMany()
                  .HasForeignKey(i => i.AssignedToId)
                  .OnDelete(DeleteBehavior.SetNull); // if user deleted, keep item
        });

        // --- ProjectMember ---
        builder.Entity<ProjectMember>(entity =>
        {
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Role).HasMaxLength(32);

            entity.HasOne(pm => pm.Project)
                  .WithMany(p => p.Members)
                  .HasForeignKey(pm => pm.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pm => pm.User)
                  .WithMany()
                  .HasForeignKey(pm => pm.UserId)
                  .OnDelete(DeleteBehavior.Restrict); // avoid cascade path
        });
    }
}