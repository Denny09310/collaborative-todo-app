using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Entities;
using Shared.Models;

namespace Server.Features.Todos;

public static class Endpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/todos")
            .WithTags("Todos")
            .RequireAuthorization();

        group.MapGet("/", GetTodos);
        group.MapGet("/{id}", GetTodoById);
        group.MapPost("/", CreateTodo);
        group.MapPut("/{id}", UpdateTodo);
        group.MapDelete("/{id}", DeleteTodo);
    }

    private static async Task<Created<TodoResponse>> CreateTodo(CreateTodoRequest dto, ApplicationDbContext db, CancellationToken ct)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            ProjectId = dto.ProjectId,
            AssignedUserId = dto.AssignedUserId,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync(ct);

        var resultDto = new TodoResponse(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.DueDate,
            todo.Priority,
            todo.CreatedAt,
            todo.LastUpdatedAt,
            todo.ProjectId,
            todo.AssignedUserId
        );

        return TypedResults.Created($"/todos/{todo.Id}", resultDto);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTodo(string id, ApplicationDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos.FindAsync([id], ct);
        if (todo is null)
            return TypedResults.NotFound();

        db.Todos.Remove(todo);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<TodoResponse>, NotFound>> GetTodoById(string id, ApplicationDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TodoResponse(
                t.Id,
                t.Title,
                t.Description,
                t.IsCompleted,
                t.DueDate,
                t.Priority,
                t.CreatedAt,
                t.LastUpdatedAt,
                t.ProjectId,
                t.AssignedUserId
            ))
            .FirstOrDefaultAsync(ct);

        return todo is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(todo);
    }

    private static async Task<Ok<GetTodosResponse>> GetTodos(ApplicationDbContext db, CancellationToken ct)
    {
        var todos = await db.Todos
            .AsNoTracking()
            .Select(t => new TodoResponse(
                t.Id,
                t.Title,
                t.Description,
                t.IsCompleted,
                t.DueDate,
                t.Priority,
                t.CreatedAt,
                t.LastUpdatedAt,
                t.ProjectId,
                t.AssignedUserId
            ))
            .ToListAsync(ct);

        return TypedResults.Ok(new GetTodosResponse(todos));
    }

    private static async Task<Results<Ok<TodoResponse>, NotFound>> UpdateTodo(string id, UpdateTodoRequest dto, ApplicationDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos.FindAsync([id], ct);
        if (todo is null)
            return TypedResults.NotFound();

        todo.Title = dto.Title;
        todo.Description = dto.Description;
        todo.IsCompleted = dto.IsCompleted;
        todo.DueDate = dto.DueDate;
        todo.Priority = dto.Priority;
        todo.AssignedUserId = dto.AssignedUserId;
        todo.LastUpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        var resultDto = new TodoResponse(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.DueDate,
            todo.Priority,
            todo.CreatedAt,
            todo.LastUpdatedAt,
            todo.ProjectId,
            todo.AssignedUserId
        );

        return TypedResults.Ok(resultDto);
    }
}