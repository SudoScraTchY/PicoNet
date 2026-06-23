namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record UserContext(Guid UserId);
// for now its only contains a userId, but we will need more claims and etc...