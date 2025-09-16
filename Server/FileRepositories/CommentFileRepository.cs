using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 0;
        comment.Id = maxId + 1;
        comments.Add(comment);
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        Comment? existingComment = comments.FirstOrDefault(c => c.Id == comment.Id);
        if (existingComment == null)
        {
            throw new InvalidOperationException($"Comment with ID {comment.Id} not found");
        }
        comments.Remove(existingComment);
        comments.Add(comment);
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        Comment? commentToDelete = comments.FirstOrDefault(c => c.Id == id);
        if (commentToDelete == null)
        {
            throw new InvalidOperationException($"Comment with ID {id} not found");
        }
        comments.Remove(commentToDelete);
        commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        Comment? comment = comments.FirstOrDefault(c => c.Id == id);
        if (comment == null)
        {
            throw new InvalidOperationException($"Comment with ID {id} not found");
        }
        return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        string commentsAsJson = File.ReadAllText(filePath); // Synkron læsning
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        return comments.AsQueryable();
    }
}