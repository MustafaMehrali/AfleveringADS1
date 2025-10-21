using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using Entities;
using ApiContracts.DTOs;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Samme stil som dit eksempel: "userRepo"
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // ---------- Helpers ----------
    private static UserDto ToDto(User u) => new UserDto
    {
        Id = u.Id,
        Username = u.Username
    };

    private async Task VerifyUserNameIsAvailableAsync(string userName)
    {
        // simpelt tjek; gør den case-insensitive hvis du vil:
        var exists = userRepo.GetMany().Any(u => u.Username == userName);
        if (exists)
        {
            // Her kan du også vælge: return Conflict("Username is already taken");
            // men for at holde stilen simpel lader vi den kaste og håndterer evt. globalt
            throw new System.InvalidOperationException("Username is already taken");
        }
    }

    // ---------- Create ----------
    // POST /api/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        await VerifyUserNameIsAvailableAsync(request.UserName);

        // Brug object-initializer; din User har ikke en (string,string)-ctor
        var user = new User
        {
            Username = request.UserName,
            Password = request.Password // OBS: i praksis bør du hashe password
        };

        var created = await userRepo.AddAsync(user);

        var dto = new UserDto
        {
            Id = created.Id,
            Username = created.Username
        };

        // Samme stil: Created med streng-URL. (Din route er /api/users/{id})
        return Created($"/api/users/{dto.Id}", dto);
        // Alternativ (REST-best practice): return CreatedAtAction(nameof(GetSingle), new { id = dto.Id }, dto);
    }

    // ---------- Read (single) ----------
    // GET /api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingle(int id)
    {
        User user;
        try
        {
            user = await userRepo.GetSingleAsync(id);
        }
        catch
        {
            return NotFound();
        }

        return Ok(ToDto(user));
    }

    // ---------- Read (many) ----------
    // GET /api/users?userName=ann&skip=0&take=50
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany(
        [FromQuery] string? userName,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        if (take is < 1 or > 200)
            return BadRequest("take must be between 1 and 200.");

        var query = userRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var u = userName.Trim();
            // Hvis din LINQ-provider ikke understøtter OrdinalIgnoreCase, brug .ToLower() på begge sider
            query = query.Where(x => x.Username.Contains(u, System.StringComparison.OrdinalIgnoreCase));
        }

        var users = query
            .OrderBy(x => x.Id)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Ok(users.Select(ToDto));
    }

    // ---------- Update ----------
    // PUT /api/users/{id}
    // Vi bruger UserDto som input for at holde samme stil (UserName uden Password)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto request)
    {
        if (id != request.Id)
            return BadRequest("Route id and body id must match.");

        User existing;
        try
        {
            existing = await userRepo.GetSingleAsync(id);
        }
        catch
        {
            return NotFound();
        }

        if (!string.Equals(existing.Username, request.UserName))
        {
            await VerifyUserNameIsAvailableAsync(request.UserName);
        }

        existing.Username = request.UserName;

        await userRepo.UpdateAsync(existing);
        return NoContent();
    }
    // ---------- Delete ----------
    // DELETE /api/users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);
        }
        catch
        {
            return NotFound();
        }

        return NoContent();
    }
}
