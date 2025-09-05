using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints(options =>
{
    options.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All);
});
builder.Services.SwaggerDocument(options =>
{
    options.ShortSchemaNames = true;
});

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthorization();
builder.Services.AddAuthenticationJwtBearer(options =>
{
    options.SigningKey = builder.Configuration["Authentication:Jwt:Key"];
});

builder.Services.Configure<JwtCreationOptions>(options =>
{
    options.SigningKey = builder.Configuration["Authentication:Jwt:Key"]!;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwaggerGen();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseMiddleware<JwtRevocationService>();

app.UseAuthentication();
app.UseAuthorization();

app.MapFastEndpoints(options =>
{
    options.Endpoints.RoutePrefix = "api";
    options.Endpoints.ShortNames = true;
    options.Errors.UseProblemDetails();
});
app.MapFallbackToFile("index.html");

app.Run();