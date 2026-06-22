namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record ValidateRegistrationRequest(string Email,string Token);