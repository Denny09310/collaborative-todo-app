using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Entities;
using System.Security.Claims;

namespace Server.Services;

public class RefreshTokenService : RefreshTokenService<TokenRequest, TokenResponse>
{
    private readonly ApplicationDbContext db;

    public RefreshTokenService(IConfiguration config, ApplicationDbContext db)
    {
        this.db = db;

        Setup(o =>
        {
            o.TokenSigningKey = config["TokenSigningKey"];
            o.AccessTokenValidity = TimeSpan.FromMinutes(5);
            o.RefreshTokenValidity = TimeSpan.FromHours(4);

            o.Endpoint("/authentication/refresh-token", ep =>
            {
                ep.Description(builder => builder
                    .Produces<TokenResponse>()
                    .ProducesProblemDetails()
                    .WithName("RefreshToken"));
            });
        });
    }

    public override async Task PersistTokenAsync(TokenResponse response)
    {
        db.Tokens.Add(new Token
        {
            UserId = response.UserId,
            Purpose = nameof(TokenResponse.AccessToken),
            Value = response.AccessToken,
            ExpireAt = response.AccessExpiry,
        });

        db.Tokens.Add(new Token
        {
            UserId = response.UserId,
            Purpose = nameof(TokenResponse.RefreshToken),
            Value = response.RefreshToken,
            ExpireAt = response.RefreshExpiry,
        });

        await db.SaveChangesAsync();
    }

    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        var token = await db.Tokens
            .FirstOrDefaultAsync(x => x.Purpose == nameof(TokenRequest.RefreshToken)
                                   && x.UserId == req.UserId);

        if (token is null || token.ExpireAt <= DateTime.UtcNow)
            AddError(r => r.RefreshToken, "Refresh token is invalid!");
    }

    public override async Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
    {
        var user = await db.Users.FindAsync([request.UserId]);

        if (user == null)
        {
            ThrowError("User not found.");
        }

        privileges.Roles.Add("User");
        privileges.Claims.Add(new(ClaimTypes.NameIdentifier, user.Id));
        privileges.Claims.Add(new(ClaimTypes.Name, user.Username));
        privileges.Claims.Add(new(ClaimTypes.Email, user.Email));
    }
}