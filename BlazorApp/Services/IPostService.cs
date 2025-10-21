using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public interface IPostService
{
    Task<Post> CreateAsync(CreatePostDto request);
    Task<Post> GetSingleAsync(int id, bool includeComments = false, bool includeAuthor = false);
    Task<IEnumerable<Post>> GetManyAsync(int? userId = null, string? search = null, int skip = 0, int take = 50, bool includeComments = false, bool includeAuthor = false);
    Task UpdateAsync(int id, UpdatePostDto request);
    Task DeleteAsync(int id);
}