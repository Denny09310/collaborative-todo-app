using Shared.Models;
using System.Net.Http.Json;

namespace Client.Services;

public class TodosApiClient(IHttpClientFactory factory)
{
    private readonly HttpClient http = factory.CreateClient("Auth");

    public async Task<GetTodosResponse?> GetTodosAsync(CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<GetTodosResponse>("api/todos", ct);
    }

    public async Task<TodoResponse?> GetTodoByIdAsync(string id, CancellationToken ct = default)
    {
        return await http.GetFromJsonAsync<TodoResponse>($"api/todos/{id}", ct);
    }

    public async Task<TodoResponse?> CreateTodoAsync(CreateTodoRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/todos", request, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<TodoResponse>(cancellationToken: ct);
    }

    public async Task<TodoResponse?> UpdateTodoAsync(string id, UpdateTodoRequest request, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/todos/{id}", request, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<TodoResponse>(cancellationToken: ct);
    }

    public async Task<bool> DeleteTodoAsync(string id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"api/todos/{id}", ct);
        return response.IsSuccessStatusCode;
    }
}