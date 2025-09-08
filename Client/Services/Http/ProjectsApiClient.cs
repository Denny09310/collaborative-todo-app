using Shared.Models;
using System.Net.Http.Json;

namespace Client.Services;

public class ProjectsApiClient(IHttpClientFactory factory)
{
    private readonly HttpClient http = factory.CreateClient("Auth");

    public async Task<GetProjectsResponse?> GetProjectsAsync(CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<GetProjectsResponse>("api/projects", ct);
    }

    public async Task<ProjectResponse?> GetProjectByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<ProjectResponse>($"api/projects/{id}", ct);
    }

    public async Task<ProjectResponse?> CreateProjectAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/projects", request, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ProjectResponse>(cancellationToken: ct);
    }

    public async Task<ProjectResponse?> UpdateProjectAsync(Guid id, UpdateProjectRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/projects/{id}", request, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ProjectResponse>(cancellationToken: ct);
    }

    public async Task<bool> DeleteProjectAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"api/projects/{id}", ct);
        return response.IsSuccessStatusCode;
    }
}