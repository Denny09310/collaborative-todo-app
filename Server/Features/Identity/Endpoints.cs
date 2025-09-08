using Microsoft.AspNetCore.Identity;
using Server.Data.Entities;
using System.Security.Claims;

namespace Server.Features.Identity;

public static class Endpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/identity")
            .WithTags("Identity");

        group.MapIdentityApi<ApplicationUser>();

        group.MapPost("/logout", Logout)
             .Produces(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status401Unauthorized)
             .RequireAuthorization();

        group.MapGet("/roles", GetRoles)
             .Produces<GetRolesResponse>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status401Unauthorized)
             .RequireAuthorization();
    }

    private static IResult GetRoles(ClaimsPrincipal user)
    {
        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            var identity = (ClaimsIdentity)user.Identity;
            var roles = identity.FindAll(identity.RoleClaimType)
                .Select(c =>
                    new GetRolesResponse.Role
                    (
                        c.Issuer,
                        c.OriginalIssuer,
                        c.Type,
                        c.Value,
                        c.ValueType
                    ));

            return TypedResults.Json(new GetRolesResponse
            {
                Roles = roles
            });
        }

        return Results.Unauthorized();
    }

    private static async Task<IResult> Logout(SignInManager<ApplicationUser> signInManager, object empty)
    {
        if (empty is not null)
        {
            await signInManager.SignOutAsync();

            return Results.Ok();
        }

        return Results.Unauthorized();
    }
}

public sealed class GetRolesResponse
{
    public IEnumerable<Role> Roles { get; set; } = [];

    public sealed record Role(
        string Issuer,
        string OriginalIssuer,
        string Type,
        string Value,
        string ValueType
    );
};