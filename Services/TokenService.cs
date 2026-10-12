using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LimasHotel.Api.Data;
using LimasHotel.Api.Models;
using LimasHotel.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace LimasHotel.Api.Services;

public class TokenService(IOptions<JwtSettings> options, MongoContext context) : ITokenService
{
    private readonly JwtSettings _settings = options.Value;
    private readonly IMongoCollection<RevokedToken> _revoked = context.RevokedTokens;

    public (string Token, DateTime ExpiresAt) Generate(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public Task RevokeAsync(string jti, DateTime expiresAt) =>
        _revoked.ReplaceOneAsync(
            t => t.Jti == jti,
            new RevokedToken { Jti = jti, ExpiresAt = expiresAt },
            new ReplaceOptions { IsUpsert = true });

    public Task<bool> IsRevokedAsync(string jti) =>
        _revoked.Find(t => t.Jti == jti).AnyAsync();
}
