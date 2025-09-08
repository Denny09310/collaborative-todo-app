using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Server.Data.Entities;
using System.Security.Claims;

namespace Server.Features.Identity;

public static class Endpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/account")
            .WithTags("Account");

        group.MapIdentityApi<ApplicationUser>();

        group.MapGet("/external-login", ExternalLogin)
             .Produces(StatusCodes.Status302Found);

        group.MapGet("/external-login/callback", ExternalLoginCallback)
             .Produces(StatusCodes.Status302Found)
             .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/logout", Logout)
             .Produces(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status401Unauthorized)
             .RequireAuthorization();

        group.MapGet("/roles", GetRoles)
             .Produces<IEnumerable<RoleClaim>>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status401Unauthorized)
             .RequireAuthorization();
    }

    private static IResult ExternalLogin(
        string provider,
        string returnUrl,
        SignInManager<ApplicationUser> signInManager)
    {
        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider,
            $"/account/external-login/callback?returnUrl={returnUrl}");

        return Results.Challenge(properties, [provider]);
    }

    private static async Task<IResult> ExternalLoginCallback(
        string returnUrl,
        HttpContext context,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        // Get the external login info from the Identity external cookie
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            // Possibly the external cookie expired or provider returned an error.
            return Results.BadRequest(new { error = "Error reading external login info." });
        }

        // Try to sign in with the external login (if already linked to a user)
        var signInResult = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            // Optional: update stored external tokens (refresh tokens / access tokens)
            await signInManager.UpdateExternalAuthenticationTokensAsync(info);

            // Clear the external cookie
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            // Option A: Redirect to returnUrl if this is a browser flow
            return Results.LocalRedirect($"~/{returnUrl}");
        }

        if (signInResult.IsLockedOut)
        {
            // Clear the external cookie
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            // Option A: Redirect to returnUrl if this is a browser flow
            return Results.LocalRedirect("~/account/lockout");
        }

        // Not linked: create a user or prompt for linking
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        // If your app requires email and the provider didn't supply it, you should prompt the user
        if (string.IsNullOrEmpty(email))
        {
            // For APIs you might return a specific response telling client to collect email
            return Results.BadRequest(new { error = "External provider did not provide an email. Please collect email and call 'external/link'." });
        }

        // Option: try to find by email (auto-link) OR create a new user
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                // map other claims if desired:
                // FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                // LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
            };

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                // Return errors to client
                return Results.BadRequest(new { errors = createResult.Errors.Select(e => e.Description) });
            }
        }

        // Link the external login to the user account
        var addLoginResult = await userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded)
        {
            return Results.BadRequest(new { errors = addLoginResult.Errors.Select(e => e.Description) });
        }

        // Persist external tokens (optional)
        await signInManager.UpdateExternalAuthenticationTokensAsync(info);

        // Sign in the user
        await signInManager.SignInAsync(user, isPersistent: false);

        // Clear the external cookie
        await context.SignOutAsync(IdentityConstants.ExternalScheme);

        // Redirect to the original return URL
        return Results.LocalRedirect($"~/{returnUrl}");
    }

    private static IResult GetRoles(ClaimsPrincipal user)
    {
        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            var identity = (ClaimsIdentity)user.Identity;
            var roles = identity.FindAll(identity.RoleClaimType)
                .Select(c =>
                    new RoleClaim
                    (
                        c.Issuer,
                        c.OriginalIssuer,
                        c.Type,
                        c.Value,
                        c.ValueType
                    ));

            return TypedResults.Json(roles);
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

public sealed record RoleClaim(
    string Issuer,
    string OriginalIssuer,
    string Type,
    string Value,
    string ValueType
);