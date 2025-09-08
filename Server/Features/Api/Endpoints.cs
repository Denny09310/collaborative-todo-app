using Server.Features.Identity;

namespace Microsoft.Extensions.Hosting;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapIdentityEndpoints();

        var group = builder.MapGroup("/api");
        return group;
    }
}
