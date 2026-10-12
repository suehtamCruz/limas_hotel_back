using LimasHotel.Api.Dtos;
using LimasHotel.Api.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace LimasHotel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await userService.GetAllAsync();
        return Ok(users.Select(UserResponse.From));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(string id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(UserResponse.From(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request)
    {
        try
        {
            var user = await userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, UserResponse.From(user));
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new { message = "Email já cadastrado." });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> Update(string id, UpdateUserRequest request)
    {
        try
        {
            var user = await userService.UpdateAsync(id, request);
            return user is null ? NotFound() : Ok(UserResponse.From(user));
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new { message = "Email já cadastrado." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) =>
        await userService.DeleteAsync(id) ? NoContent() : NotFound();
}
