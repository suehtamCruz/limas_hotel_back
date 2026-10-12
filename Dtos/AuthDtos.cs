using System.ComponentModel.DataAnnotations;

namespace LimasHotel.Api.Dtos;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record LoginResponse(string Token, DateTime ExpiresAt, UserResponse User);
