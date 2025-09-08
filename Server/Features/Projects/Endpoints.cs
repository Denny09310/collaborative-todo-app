using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Features.Projects;

public static class Endpoints
{
    public static void MapProjectsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/projects")
            .WithTags("Projects")
            .RequireAuthorization();

        group.MapGet("/", GetProjects);
    }

    private static async Task<IResult> GetProjects(ApplicationDbContext db, CancellationToken ct)
    {
        var groups = await db.Projects.ToListAsync(ct);
        return Results.Ok(groups);
    }
}