using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;


public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new List<Comment>();
    public CommentInMemoryRepository()
    {
        // Dummy data
        comments.Add(new Comment { Id = 1, Body = "God post!", UserId = 2, PostId = 1 });
        comments.Add(new Comment { Id = 2, Body = "Det giver mening.", UserId = 1, PostId = 2 });
        comments.Add(new Comment { Id = 3, Body = "Tak for svaret.", UserId = 3, PostId = 3 });
    }
    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any() ? comments.Max(c => c.Id) + 1 : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        var existing = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existing is null)
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");

        comments.Remove(existing);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var toRemove = comments.SingleOrDefault(c => c.Id == id);
        if (toRemove is null)
            throw new InvalidOperationException($"Comment with ID '{id}' not found");

        comments.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        var comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
            throw new InvalidOperationException($"Comment with ID '{id}' not found");

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany() => comments.AsQueryable();
}
    
