namespace PicoNet.Contracts.DTOs.Responses.Auth;

public record AuthTokenResponse(string AccessToken, DateTime AccessExpiresAt, string RefreshToken,DateTime RefreshExpiresAt);