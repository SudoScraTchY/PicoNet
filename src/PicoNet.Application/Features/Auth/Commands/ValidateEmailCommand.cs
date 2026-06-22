using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Auth.Commands;

public record ValidateEmailCommand(string Email,string Token,UserAgentData UserAgentData);