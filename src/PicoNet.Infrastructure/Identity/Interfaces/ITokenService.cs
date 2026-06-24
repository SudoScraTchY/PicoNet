using System.Security.Claims;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Infrastructure.Identity.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user);

    Task<(string RefreshToken, DateTime ExpiresAt)> GenerateRefreshTokenAsync(
        ApplicationUser user, string? createdByIp, string? userAgent, Guid? replacesTokenId, CancellationToken ct);
    
    Task<ClaimsPrincipal?> ValidateRefreshTokenAsync(string token);
}