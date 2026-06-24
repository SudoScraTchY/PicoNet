using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Auth.Commands;

public record RegisterCommand(string Username,string Email, string Password,UserAgentData UserAgentData);