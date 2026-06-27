namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record UserContext(Guid UserId, IReadOnlyList<string> Roles)
{
    public bool IsAdmin => Roles.Contains("Admin");
}
// for now its only contains a userId, but we will need more claims and etc...