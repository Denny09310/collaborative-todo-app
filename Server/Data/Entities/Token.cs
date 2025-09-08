namespace Server.Data.Entities;

public class Token
{
    public string Id { get; set; } = default!;
    public string Value { get; set; }= default!;
    public string Purpose { get; set; } = default!;

    public DateTime ExpireAt { get; set; }

    public bool IsRevoked { get; set; }

    #region Navigation Properties

    public string UserId { get; set; } = default!;
    public User User { get; set; } = default!;

    #endregion
}

