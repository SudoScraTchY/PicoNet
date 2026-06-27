namespace PicoNet.Contracts.DTOs.Responses.Roles;

public record RoleResponse
{
    public Guid Id { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public int UserCount { get; init; }
}

