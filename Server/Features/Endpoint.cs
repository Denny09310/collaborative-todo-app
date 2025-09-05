using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Server.Features;

[HttpGet("/hello")]
[AllowAnonymous]
public class Endpoint : EndpointWithoutRequest<Response>
{
    public override Task HandleAsync(CancellationToken ct)
    {
        Response.Message = "Hello world!";
        return Send.OkAsync(Response, ct);
    }
}

public sealed class Response
{
    public string Message { get; set; }
}
