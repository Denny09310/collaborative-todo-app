namespace Shared.Models;

public sealed record TodoResponse(
    string Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDate,
    int Priority,
    DateTime CreatedAt,
    DateTime LastUpdatedAt,
    string ProjectId,
    string? AssignedUserId
);
