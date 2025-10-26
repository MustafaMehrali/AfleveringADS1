using ApiContracts.DTOs;

namespace BlazorApp.Services;

public interface IUserService
{
    Task<UserDto> AddUserAsync(CreateUserDto request);
    Task<UserDto> GetSingleAsync(int id);
    Task<IEnumerable<UserDto>> GetManyAsync(string? userName = null, int skip = 0, int take = 50);
    Task<IEnumerable<UserDto>> GetUsersAsync(); // Convenience method for getting all users
    Task UpdateUserAsync(int id, UpdateUserDto request);
    Task DeleteUserAsync(int id);
}
