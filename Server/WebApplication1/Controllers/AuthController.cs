using ApiContracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public AuthController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        // Find user by username
        var user = userRepository.GetMany()
            .FirstOrDefault(u => u.Username == request.Username);

        if (user == null)
        {
            return Unauthorized("User not found");
        }

        // Check password
        if (user.Password != request.Password)
        {
            return Unauthorized("Incorrect password");
        }

        // Convert to UserDto (without password)
        UserDto userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username
        };

        return Ok(userDto);
    }
}
