using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Entities;
using Server.Services;
using System.Security.Claims;

namespace Server.Features.Authentication.Login;

public sealed class Endpoint(ApplicationDbContext db) : Endpoint<Request, TokenResponse>
{
    private static readonly PasswordHasher<User> hasher = new();

    public override void Configure()
    {
        Post("/authentication/login");
        AllowAnonymous();

        Description(builder => builder
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .ProducesProblemDetails()
            .WithName("Login"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == req.Email, ct);

        if (user is null)
        {
            ThrowError("Invalid credentials.", StatusCodes.Status401Unauthorized);
        }

        var result = hasher.VerifyHashedPassword(user, user.Password, req.Password);

        if (result is PasswordVerificationResult.Failed)
        {
            ThrowError("Invalid credentials.", StatusCodes.Status401Unauthorized);
        }

        Response = await CreateTokenWith<RefreshTokenService>(user.Id, u =>
        {
            u.Roles.AddRange(["User"]);
            u.Claims.Add(new(ClaimTypes.NameIdentifier, user.Id));
            u.Claims.Add(new(ClaimTypes.Name, user.Username));
            u.Claims.Add(new(ClaimTypes.Email, user.Email));
        });
    }
}

public sealed record Request(string Email, string Password);