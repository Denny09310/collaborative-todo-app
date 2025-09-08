using Client.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApiClientsRegistration
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        services.AddScoped<ProjectsApiClient>();
        services.AddScoped<TodosApiClient>();
        // Add more here later (e.g. AuthApiClient, UserApiClient, etc.)

        return services;
    }
}