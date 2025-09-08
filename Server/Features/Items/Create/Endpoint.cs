using FastEndpoints;
using Server.Data;
using Server.Data.Entities;

namespace Server.Features.Items.Create;

public sealed class Endpoint(ApplicationDbContext db) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/items/create");
        Roles("User");

        Description(builder => builder
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesProblemDetails()
            .WithName("CreateItem"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var item = new TodoItem
        {
            Title = req.Title,
            Description = req.Description,
            DueDate = req.DueDate,
            Priority = req.Priority,
            ProjectId = req.ProjectId,
            AssignedToId = req.AssignedToId
        };

        db.Items.Add(item);
        await db.SaveChangesAsync(ct);

        Response = new Response(item.Id, item.Title, item.IsCompleted);
        await Send.OkAsync(Response, ct);
    }
}

public sealed record Request(
    string ProjectId,
    string Title,
    string Description,
    DateTime? DueDate,
    int Priority,
    string? AssignedToId);

public sealed record Response(string Id, string Title, bool IsCompleted);
