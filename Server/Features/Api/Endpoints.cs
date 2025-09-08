using Server.Features.Identity;

namespace Microsoft.Extensions.Hosting;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api");
        group.MapIdentityEndpoints();
        return group;
    }
}
