namespace PicoNet.Contracts.DTOs.Responses.Users;

public record UserResponse
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool EmailConfirmed { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public bool LockoutEnabled { get; init; }
    public int AccessFailedCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<string> Roles { get; init; } = new();
}

