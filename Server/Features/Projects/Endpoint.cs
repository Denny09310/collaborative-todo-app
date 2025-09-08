using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using System.Security.Claims;

namespace Server.Features.Projects;

public sealed class Endpoint(ApplicationDbContext db) : EndpointWithoutRequest<Response>
{
    public override void Configure()
    {
        Get("/projects");
        Roles("User");

        Description(b => b
            .Produces<Response>()
            .ProducesProblemDetails()
            .WithName("GetProjects"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            ThrowError("Unauthorized.", StatusCodes.Status401Unauthorized);

        var projects = await db.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .Include(p => p.Items)
            .OrderByDescending(p => p.LastUpdatedAt)
            .Select(p => new Response.Project
            (
                p.Id,
                p.Name,
                p.Description,
                p.Items.OrderBy(i => i.IsCompleted)
                    .ThenBy(i => i.DueDate ?? DateTime.MaxValue)
                    .Select(i => new Response.Item
                    (
                        i.Id,
                        i.Title,
                        i.IsCompleted,
                        i.DueDate,
                        i.Priority,
                        i.AssignedTo!.Id,
                        i.AssignedTo!.Username
                    ))
                    .ToList()
            ))
            .ToListAsync(ct);

        Response = new Response(projects);
        await Send.OkAsync(Response, ct);
    }
}

public sealed record Response(IEnumerable<Response.Project> Projects)
{
    public sealed record Project(string Id, string Name, string Description, List<Item> Items);
    public sealed record Item(string Id, string Title, bool IsCompleted, DateTime? DueDate, int Priority, string? AssignedToId, string AssignedToUsername);
}