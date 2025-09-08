namespace Server.Data.Entities;

public class Todo
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }

    public bool IsCompleted { get; set; } = false;
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 0; // 0=Low, 1=Medium, 2=High

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    #region Navigation Properties

    // Each todo belongs to a project
    public string ProjectId { get; set; } = default!;
    public Project Project { get; set; } = default!;

    // (Optional) assign a todo to a specific user
    public string? AssignedUserId { get; set; }
    public ApplicationUser? AssignedUser { get; set; }

    #endregion
}
