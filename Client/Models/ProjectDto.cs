namespace Client.Models;

public sealed record ProjectDto(string Id, string Name, string Description, List<ItemDto> Items);
