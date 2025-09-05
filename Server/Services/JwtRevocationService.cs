using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Services;

public class JwtRevocationService(IDbContextFactory<ApplicationDbContext> factory, RequestDelegate next) : JwtRevocationMiddleware(next)
{
    protected override async Task<bool> JwtTokenIsValidAsync(string jwtToken, CancellationToken ct)
    {
        using var db = await factory.CreateDbContextAsync(ct);
        var token = await db.Tokens.FirstOrDefaultAsync(x => x.Value == jwtToken, ct);
        return token?.IsRevoked == false;
    }
}