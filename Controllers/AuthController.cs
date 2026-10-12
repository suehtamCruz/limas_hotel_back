using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LimasHotel.Api.Dtos;
using LimasHotel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LimasHotel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await userService.GetByEmailAsync(request.Email);
        if (user is null || !userService.VerifyPassword(user, request.Password))
            return Unauthorized(new { message = "Email ou senha inválidos." });

        var (token, expiresAt) = tokenService.Generate(user);
        return Ok(new LoginResponse(token, expiresAt, UserResponse.From(user)));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var exp = User.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (jti is null || !long.TryParse(exp, out var expUnix))
            return BadRequest(new { message = "Token inválido." });

        await tokenService.RevokeAsync(jti, DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var user = id is null ? null : await userService.GetByIdAsync(id);
        return user is null ? Unauthorized() : Ok(UserResponse.From(user));
    }
}
