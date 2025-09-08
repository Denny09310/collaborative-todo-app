namespace Server.Data.Entities;

public class TodoItem
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 0; // 0=Low, 1=Medium, 2=High
    public bool IsCompleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    #region Navigation Properties

    public string ProjectId { get; set; } = default!;
    public TodoProject Project { get; set; } = default!;

    public string? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    #endregion Navigation Properties
}

