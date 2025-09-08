namespace Server.Data.Entities;

public class Project
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    #region Navigation Properties

    // One project can have many todos
    public ICollection<Todo> Todos { get; set; } = new List<Todo>();

    // Collaboration: many users can belong to a project
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

    #endregion
}
