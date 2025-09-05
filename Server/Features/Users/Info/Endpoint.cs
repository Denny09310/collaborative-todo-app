using FastEndpoints;
using Server.Data;
using System.Security.Claims;

namespace Server.Features.Users.Info;

public class Endpoint(ApplicationDbContext db) : EndpointWithoutRequest<Response>
{
    public override void Configure()
    {
        Get("/users/info");

        Description(builder => builder
            .Produces<Response>()
            .ProducesProblemDetails(StatusCodes.Status401Unauthorized)
            .WithName("GetUserInfo"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            ThrowError("You are not authorized.", StatusCodes.Status401Unauthorized);
        }

        var user = await db.Users.FindAsync([userId], ct);

        if (user is null)
        {
            ThrowError("You are not authorized.", StatusCodes.Status401Unauthorized);
        }

        Response = new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            LastUpdatedAt = user.LastUpdatedAt,
        };
    }
}

public sealed class Response
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}