using FastEndpoints;
using FastEndpoints.Swagger;
using Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddFastEndpoints(options =>
{
    options.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All);
});
builder.Services.SwaggerDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwaggerGen();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.MapFastEndpoints(options =>
{
    options.Endpoints.RoutePrefix = "api";
});
app.MapFallbackToFile("index.html");

app.Run();