namespace PicoNet.Contracts.DTOs.Responses.Auth;

public record AuthResponse(string AccessToken, string RefreshToken,DateTime ExpiresAt,AuthResponseUser  User);