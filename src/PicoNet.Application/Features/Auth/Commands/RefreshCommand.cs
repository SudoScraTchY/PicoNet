using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Auth.Commands;

public record RefreshCommand(UserAgentData  UserAgentData,
    UserContext? UserContext,string RefreshToken);