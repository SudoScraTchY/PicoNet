namespace PicoNet.Contracts.DTOs.Responses.Users;

public record UserRolesResponse
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
}

