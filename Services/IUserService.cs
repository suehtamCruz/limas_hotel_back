using LimasHotel.Api.Dtos;
using LimasHotel.Api.Models;

namespace LimasHotel.Api.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(CreateUserRequest request);
    Task<User?> UpdateAsync(string id, UpdateUserRequest request);
    Task<bool> DeleteAsync(string id);
    bool VerifyPassword(User user, string password);
}
