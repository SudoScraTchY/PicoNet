using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Auth.Commands;

public record ChangeEmailCommand(UserContext UserContext, string NewEmail,UserAgentData UserAgentData);
