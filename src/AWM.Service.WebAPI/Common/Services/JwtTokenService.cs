using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Interfaces;
using AWM.Service.WebAPI.Common.Settings;
using AWM.Service.WebAPI.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AWM.Service.WebAPI.Common.Services;

/// <summary>
/// Implementation of JWT token generation service.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <inheritdoc/>
    public string GenerateToken(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Email, user.Email),
            new(AuthorizationConstants.UniversityIdClaimType, user.UniversityId.ToString())
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(AuthorizationConstants.RoleClaimType, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc/>
    public (string Token, DateTime Expiry) GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);
            var expiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            return (token, expiry);
        }
    }
}
