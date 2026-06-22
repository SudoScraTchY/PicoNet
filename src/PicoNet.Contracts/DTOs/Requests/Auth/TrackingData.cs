namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record UserAgentData(string? UserAgent, string? IpAddress = null, string? Referrer = null);