using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using System.Security.Claims;

namespace Server.Features.Authentication.Logout;

public class Endpoint(ApplicationDbContext db) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/authentication/logout");

        Description(builder => builder
            .ProducesProblemDetails(StatusCodes.Status401Unauthorized)
            .WithName("Logout"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            ThrowError("You are not authorized.", StatusCodes.Status401Unauthorized);
        }

        await db.Tokens
            .Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(
                set => set.SetProperty(p => p.IsRevoked, true),
                ct);
    }
}