using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Entities;
using Shared.Models;

namespace Server.Features.Projects;

public static class Endpoints
{
    public static void MapProjectsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/projects")
            .WithTags("Projects")
            .RequireAuthorization();

        group.MapGet("/", GetProjects);
        group.MapGet("/{id}", GetProjectById);
        group.MapPost("/", CreateProject);
        group.MapPut("/{id}", UpdateProject);
        group.MapDelete("/{id}", DeleteProject);
    }

    private static async Task<Created<ProjectResponse>> CreateProject(CreateProjectRequest dto, ApplicationDbContext db, CancellationToken ct)
    {
        var project = new Project { Name = dto.Name, Description = dto.Description };
        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/projects/{project.Id}",
            new ProjectResponse(project.Id, project.Name, project.Description));
    }

    private static async Task<Results<NoContent, NotFound>> DeleteProject(Guid id, ApplicationDbContext db, CancellationToken ct)
    {
        var project = await db.Projects.FindAsync([id], ct);
        if (project is null)
            return TypedResults.NotFound();

        db.Projects.Remove(project);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<ProjectResponse>, NotFound>> GetProjectById(string id, ApplicationDbContext db, CancellationToken ct)
    {
        var project = await db.Projects
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProjectResponse(p.Id, p.Name, p.Description))
            .FirstOrDefaultAsync(ct);

        return project is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(project);
    }

    private static async Task<Ok<GetProjectsResponse>> GetProjects(ApplicationDbContext db, CancellationToken ct)
    {
        var projects = await db.Projects
            .AsNoTracking()
            .Select(p => new ProjectResponse(p.Id, p.Name, p.Description))
            .ToListAsync(ct);

        return TypedResults.Ok(new GetProjectsResponse(projects));
    }

    private static async Task<Results<Ok<ProjectResponse>, NotFound>> UpdateProject(string id, UpdateProjectRequest dto, ApplicationDbContext db, CancellationToken ct)
    {
        var project = await db.Projects.FindAsync([id], ct);
        if (project is null)
            return TypedResults.NotFound();

        project.Name = dto.Name;
        project.Description = dto.Description;
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(new ProjectResponse(project.Id, project.Name, project.Description));
    }
}