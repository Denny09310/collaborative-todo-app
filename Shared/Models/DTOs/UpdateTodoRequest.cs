namespace Shared.Models;

public sealed record UpdateTodoRequest(
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? DueDate,
    int Priority,
    string? AssignedUserId
);