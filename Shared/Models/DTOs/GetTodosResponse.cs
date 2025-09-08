namespace Shared.Models;

public sealed record GetTodosResponse(IEnumerable<TodoResponse> Todos);
