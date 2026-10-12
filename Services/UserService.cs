using LimasHotel.Api.Data;
using LimasHotel.Api.Dtos;
using LimasHotel.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LimasHotel.Api.Services;

public class UserService(MongoContext context) : IUserService
{
    private readonly IMongoCollection<User> _users = context.Users;

    public Task<List<User>> GetAllAsync() =>
        _users.Find(_ => true).ToListAsync();

    public async Task<User?> GetByIdAsync(string id) =>
        ObjectId.TryParse(id, out _)
            ? await _users.Find(u => u.Id == id).FirstOrDefaultAsync()
            : null;

    public async Task<User?> GetByEmailAsync(string email) =>
        await _users.Find(u => u.Email == Normalize(email)).FirstOrDefaultAsync();

    public async Task<User> CreateAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Email = Normalize(request.Email),
            Name = request.Name.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<User?> UpdateAsync(string id, UpdateUserRequest request)
    {
        var user = await GetByIdAsync(id);
        if (user is null) return null;

        if (request.Email is not null) user.Email = Normalize(request.Email);
        if (request.Name is not null) user.Name = request.Name.Trim();
        if (request.Password is not null) user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.UpdatedAt = DateTime.UtcNow;

        await _users.ReplaceOneAsync(u => u.Id == id, user);
        return user;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return false;
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public bool VerifyPassword(User user, string password) =>
        BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
