using Blazored.LocalStorage;

namespace Client.Services;

public class TokenHandler(ILocalStorageService storage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await storage.GetItemAsync<string>("access_token");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}