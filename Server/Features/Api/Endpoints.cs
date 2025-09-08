using Server.Features.Identity;
using Server.Features.Projects;
using Server.Features.Todos;

namespace Microsoft.Extensions.Hosting;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapIdentityEndpoints();

        var group = builder.MapGroup("/api");
        group.MapProjectsEndpoints();
        group.MapTodoEndpoints();
        return group;
    }
}