using Riok.Mapperly.Abstractions;
using Shared.Models;

namespace Client.Models;

public class Project
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ProjectExtensions
{
    public static partial Project ToModel(this ProjectResponse dto);
    public static partial List<Project> ToList(this IEnumerable<ProjectResponse> dtos);

    public static partial CreateProjectRequest ToCreateRequest(this Project project);
    public static partial UpdateProjectRequest ToUpdateRequest(this Project project);
}