using System.ComponentModel.DataAnnotations;
using LimasHotel.Api.Models;

namespace LimasHotel.Api.Dtos;

public record CreateUserRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, MinLength(2)] string Name);

public record UpdateUserRequest(
    [EmailAddress] string? Email,
    [MinLength(6)] string? Password,
    [MinLength(2)] string? Name);

public record UserResponse(string Id, string Email, string Name, DateTime CreatedAt, DateTime UpdatedAt)
{
    public static UserResponse From(User user) =>
        new(user.Id, user.Email, user.Name, user.CreatedAt, user.UpdatedAt);
}
