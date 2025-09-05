using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Entities;

namespace Server.Features.Authentication.Register;

public sealed class Endpoint(ApplicationDbContext db) : Endpoint<Request>
{
    private static readonly PasswordHasher<User> hasher = new();

    public override void Configure()
    {
        Post("/authentication/register");
        AllowAnonymous();

        Description(builder => builder
            .ProducesProblemDetails(StatusCodes.Status409Conflict)
            .WithName("Register"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var exists = await db.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == req.Email, ct);

        if (exists)
        {
            ThrowError("Email already taken.", StatusCodes.Status409Conflict);
        }

        var user = new User
        {
            Email = req.Email,
            Username = req.Username,
        };

        user.Password = hasher.HashPassword(user, req.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }
}

public sealed record Request(string Email, string Username, string Password);