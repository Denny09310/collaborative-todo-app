using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Client.Services;

public interface IAccountManagement
{
    Task LoginAsync(string username, string password);

    Task Logout();
}

public class ServerAuthenticationStateProvider(ILocalStorageService storage, IApiClient api) : AuthenticationStateProvider, IAccountManagement
{
    private static readonly AuthenticationState _unauthorized = new(new ClaimsPrincipal());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storage.GetItemAsync<string>("access_token");

        if (string.IsNullOrEmpty(token))
        {
            return _unauthorized;
        }

        using var response = await api.GetUserInfo();

        if (!response.IsSuccessful)
        {
            return _unauthorized;
        }

        var content = response.Content;

        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, content.Id),
            new(ClaimTypes.Email, content.Email),
            new(ClaimTypes.Name, content.Username),
        };

        var identity = new ClaimsIdentity(claims, "Jwt");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    public async Task LoginAsync(string username, string password)
    {
        using var response = await api.Login(new() { Email = username, Password = password });

        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException("Invalid authentication.");
        }

        var content = response.Content;

        await storage.SetItemAsync("access_token", content.AccessToken);
        await storage.SetItemAsync("refresh_token", content.RefreshToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await api.Logout();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}