namespace PicoNet.Application.Features.Auth.Commands;

public record LoginCommand(string? Email,string? Username, string Password);