namespace Shared.Models;

public sealed record CreateProjectRequest(string Name, string? Description = null);
