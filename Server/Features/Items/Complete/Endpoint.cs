using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Features.Items.Complete;

public sealed class Endpoint(ApplicationDbContext db) : Endpoint<Request>
{
    public override void Configure()
    {
        Post("/items/complete");
        Roles("User");

        Description(builder => builder
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblemDetails()
            .WithName("CompleteItem"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var item = await db.Items.FirstOrDefaultAsync(x => x.Id == req.ItemId, ct);

        if (item is null)
        {
            ThrowError("Item not found.", StatusCodes.Status404NotFound);
        }

        item.IsCompleted = true;
        item.LastUpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}

public sealed record Request(string ItemId);