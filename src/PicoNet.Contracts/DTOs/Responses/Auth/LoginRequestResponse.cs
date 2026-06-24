namespace PicoNet.Contracts.DTOs.Responses.Auth;

public record AuthResponse(AuthTokenResponse Tokens,AuthResponseUser  User);