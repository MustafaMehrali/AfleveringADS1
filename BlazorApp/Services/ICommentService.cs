using ApiContracts.DTOs;
using Entities;

namespace BlazorApp.Services;

public interface ICommentService
{
    Task<Comment> CreateAsync(CreateCommentDto request);
    Task<Comment> GetSingleAsync(int id);
    Task<IEnumerable<Comment>> GetManyAsync(int? postId = null, int? userId = null, string? search = null, int skip = 0, int take = 100);
    Task UpdateAsync(int id, UpdateCommentDto request);
    Task DeleteAsync(int id);
    Task<IEnumerable<Comment>> GetCommentsForPostAsync(int postId);
    Task<Comment> AddCommentToPostAsync(int postId, CreateCommentDto request);
}