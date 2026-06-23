namespace PicoNet.Infrastructure.Identity.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user);
    bool ValidateRefreshToken(string token);
}