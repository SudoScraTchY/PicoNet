using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Auth.Commands;

public record ValidateEmailCommand(Guid Id,string Token,UserAgentData UserAgentData);