namespace PicoNet.Contracts.DTOs.Responses.Users;

public record ListUsersResponse
{
    public List<UserResponse> Users { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}

