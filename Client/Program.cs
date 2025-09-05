using Blazored.LocalStorage;
using Client.Components;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ServerAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ServerAuthenticationStateProvider>());
builder.Services.AddScoped<IAccountManagement>(sp => sp.GetRequiredService<ServerAuthenticationStateProvider>());

builder.Services.ConfigureRefitClients(new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddTransient<TokenHandler>();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ToastService>();

await builder.Build().RunAsync();