namespace Server.Data.Entities;

public class TodoProject
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    // Ownership
    public string OwnerId { get; set; } = default!;
    public User Owner { get; set; } = default!;

    // Navigation
    public ICollection<TodoItem> Items { get; set; } = new List<TodoItem>();
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}

