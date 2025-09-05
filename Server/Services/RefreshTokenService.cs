using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Server.Data;
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
            o.TokenSigningKey = config["Authentication:Jwt:Key"];
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
        var accessToken = await db.Tokens
            .FirstOrDefaultAsync(x => x.UserId == response.UserId
                                   && x.Purpose == nameof(TokenResponse.AccessToken));

        accessToken ??= new()
        {
            UserId = response.UserId,
            Purpose = nameof(TokenResponse.AccessToken),
        };

        accessToken.Value = response.AccessToken;
        accessToken.ExpireAt = response.AccessExpiry;
        accessToken.IsRevoked = false;

        db.Tokens.Update(accessToken);

        var refreshToken = await db.Tokens
            .FirstOrDefaultAsync(x => x.UserId == response.UserId
                                   && x.Purpose == nameof(TokenResponse.RefreshToken));

        refreshToken ??= new()
        {
            UserId = response.UserId,
            Purpose = nameof(TokenResponse.RefreshToken),
        };

        refreshToken.Value = response.RefreshToken;
        refreshToken.ExpireAt = response.RefreshExpiry;
        refreshToken.IsRevoked = false;

        db.Tokens.Update(refreshToken);

        await db.SaveChangesAsync();
    }

    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        var token = await db.Tokens
            .FirstOrDefaultAsync(x => x.Purpose == nameof(TokenRequest.RefreshToken)
                                   && x.UserId == req.UserId);

        if (token == null)
        {
            AddError(r => r.RefreshToken, "Refresh token does not exists!");
            return;
        }

        if (token.ExpireAt < DateTime.UtcNow)
        {
            AddError(r => r.RefreshToken, "Refresh token is invalid!");

            token.IsRevoked = true;

            db.Tokens.Update(token);
            await db.SaveChangesAsync();
        }
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