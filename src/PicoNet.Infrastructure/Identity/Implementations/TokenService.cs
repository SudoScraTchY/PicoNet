using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Extensions;
using PicoNet.Infrastructure.Identity.Entities;
using PicoNet.Infrastructure.Identity.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace PicoNet.Infrastructure.Identity.Implementations;

public sealed class TokenService : ITokenService
{
    //TODO We Should Change to IOptions<JwtOptions> or something
    private readonly IConfiguration _config;
    private readonly PicoNetDbContext _dbContext;

    public TokenService(IConfiguration config, PicoNetDbContext dbContext)
    {
        _config = config;
        _dbContext = dbContext;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(30); // short-lived — discuss refresh tokens later

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("token_type", "access"), 
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // unique per token
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public async Task<(string RefreshToken, DateTime ExpiresAt)> GenerateRefreshTokenAsync(ApplicationUser user, string? createdByIp, string? userAgent, Guid? replacesTokenId, CancellationToken ct)
    {
        var jwtSection = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddDays(14); // refresh tokens live much longer than access tokens
        var jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("token_type", "refresh"),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        var token = new JwtSecurityToken(jwtSection["Issuer"], jwtSection["Audience"], claims,
            expires: expiresAt, signingCredentials: credentials);
        var rawToken = new JwtSecurityTokenHandler().WriteToken(token);

        var record = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = RefreshTokenHasher.Hash(rawToken),
            ExpiresAt = expiresAt,
            CreatedByIp = createdByIp,
            UserAgent = userAgent
        };
        _dbContext.RefreshTokens.Add(record);

        if (replacesTokenId is not null)
        {
            var old = await _dbContext.RefreshTokens.FindAsync([replacesTokenId.Value], ct);
            if (old is not null)
            {
                old.RevokedAt = DateTime.UtcNow;
                old.ReplacedByTokenId = record.Id;
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return (rawToken, expiresAt);
    }

    public async Task<ClaimsPrincipal?> ValidateRefreshTokenAsync(string token)
    {
        var handler = new JsonWebTokenHandler();
        var jwtSection = _config.GetSection("Jwt");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

        var result = await handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true, ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true, ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true, ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuerSigningKey = true, IssuerSigningKey = securityKey
        });

        if (!result.IsValid) return null;

        // reject an access token presented as a refresh token — see next section
        if (result.ClaimsIdentity.FindFirst("token_type")?.Value != "refresh") return null;

        return new ClaimsPrincipal(result.ClaimsIdentity);
    }
}
