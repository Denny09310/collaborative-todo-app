namespace Server.Data.Entities;

/// <summary>
/// Many-to-many join table between User and Project.
/// </summary>
public class ProjectMember
{
    public string ProjectId { get; set; } = default!;
    public Project Project { get; set; } = default!;

    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;

    // (Optional) Role inside project: Owner, Member, etc.
    public string Role { get; set; } = "Member";
}