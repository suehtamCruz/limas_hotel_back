using LimasHotel.Api.Models;

namespace LimasHotel.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) Generate(User user);
    Task RevokeAsync(string jti, DateTime expiresAt);
    Task<bool> IsRevokedAsync(string jti);
}
