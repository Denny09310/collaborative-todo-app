namespace Shared.Models;

public sealed record CreateTodoRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    int Priority,
    string ProjectId,
    string? AssignedUserId
);
