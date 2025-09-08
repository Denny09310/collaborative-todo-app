namespace Server.Data.Entities;

public class ProjectMember
{
    public string Id { get; set; } = default!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string Role { get; set; } = "member"; // e.g. "owner", "editor", "viewer"

    #region Navigation Properties

    public TodoProject Project { get; set; } = default!;
    public string ProjectId { get; set; } = default!;

    public User User { get; set; } = default!;
    public string UserId { get; set; } = default!;

    #endregion Navigation Properties
}