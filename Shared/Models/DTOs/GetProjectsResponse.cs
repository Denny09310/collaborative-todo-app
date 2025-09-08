namespace Shared.Models;

public sealed record GetProjectsResponse(IEnumerable<ProjectResponse> Projects);
