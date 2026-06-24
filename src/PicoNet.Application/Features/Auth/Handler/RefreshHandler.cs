using System.IdentityModel.Tokens.Jwt;
using PicoNet.Application.Features.Auth.Commands;
using PicoNet.Contracts.DTOs.Responses.Auth;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PicoNet.Application.Mappings;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Extensions;
using PicoNet.Infrastructure.Identity.Entities;
using PicoNet.Infrastructure.Identity.Interfaces;

namespace PicoNet.Application.Features.Auth.Handler;

public sealed class RefreshHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly PicoNetDbContext _dbContext;

    public RefreshHandler(UserManager<ApplicationUser> userManager,  ITokenService tokenService, PicoNetDbContext dbContext)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    // We Should Build a Table for Issued Tokens alongside Identity Entities in Infrastructure
    // that way we can find the RefreshToken and manage Them each RefreshToken Represent a Session for any user
    public async Task<ErrorOr<AuthResponse>> Handle(RefreshCommand command, CancellationToken ct)
    {
        var principal = await _tokenService.ValidateRefreshTokenAsync(command.RefreshToken);
        if (principal is null)
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid or expired refresh token.");

        var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (userId is null || !Guid.TryParse(userId, out var parsedId))
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid refresh token.");

        var tokenHash = RefreshTokenHasher.Hash(command.RefreshToken);
        var stored = await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        if (stored is null)
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Token not recognized.");

        if (stored.RevokedAt is not null)
        {
            // reuse of an already-rotated token — likely theft. Nuke every active session for this user.
            await RevokeAllActiveTokensAsync(stored.UserId, ct);
            return Error.Unauthorized("Auth.TokenReuseDetected", "Session invalidated for security reasons.");
        }

        if (!stored.IsActive)
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Token expired.");

        var user = await _userManager.FindByIdAsync(parsedId.ToString());
        if (user is null)
            return Error.Unauthorized("Auth.InvalidUser", "User no longer exists.");

        var (accessToken, accessExpiresAt) = _tokenService.GenerateToken(user);
        var (newRefreshToken, refreshExpiresAt) = await _tokenService.GenerateRefreshTokenAsync(
            user, command.UserAgentData.IpAddress, command.UserAgentData.UserAgent, stored.Id, ct);

        return new AuthResponse(
            new AuthTokenResponse(accessToken, accessExpiresAt, newRefreshToken, refreshExpiresAt),
            user.ToAuthResponseUser());
    }

    private async Task RevokeAllActiveTokensAsync(Guid userId, CancellationToken ct)
    {
        var dtNow = DateTime.UtcNow;
        await _dbContext.RefreshTokens.AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive && x.ReplacedByTokenId == null && x.RevokedAt == null)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(property => property.RevokedAt, dtNow)
                    .SetProperty(property => property.IsActive, false), cancellationToken: ct);
    }
}