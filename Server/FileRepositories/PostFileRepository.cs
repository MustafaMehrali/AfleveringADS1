using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Post> AddAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;
        post.Id = maxId + 1;
        posts.Add(post);
        postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        Post? existingPost = posts.FirstOrDefault(p => p.Id == post.Id);
        if (existingPost == null)
        {
            throw new InvalidOperationException($"Post with ID {post.Id} not found");
        }
        posts.Remove(existingPost);
        posts.Add(post);
        postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        Post? postToDelete = posts.FirstOrDefault(p => p.Id == id);
        if (postToDelete == null)
        {
            throw new InvalidOperationException($"Post with ID {id} not found");
        }
        posts.Remove(postToDelete);
        postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        Post? post = posts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            throw new InvalidOperationException($"Post with ID {id} not found");
        }
        return post;
    }

    public IQueryable<Post> GetMany()
    {
        string postsAsJson = File.ReadAllText(filePath);                 // sync læsning
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson)!;
        return posts.AsQueryable();
    }

}