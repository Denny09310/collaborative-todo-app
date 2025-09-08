using FastEndpoints;
using Server.Data;
using Server.Data.Entities;

namespace Server.Features.Projects.Create;

public sealed class Endpoint(ApplicationDbContext db) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/projects/create");
        Roles("User");

        Description(builder => builder
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesProblemDetails()
            .WithName("CreateProject"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var project = new TodoProject
        {
            Name = req.Name,
            Description = req.Description,
            OwnerId = req.OwnerId
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);

        Response = new Response(project.Id, project.Name, project.Description);
        await Send.OkAsync(Response, ct);
    }
}

public sealed record Request(string OwnerId, string Name, string Description);

public sealed record Response(string Id, string Name, string Description);
