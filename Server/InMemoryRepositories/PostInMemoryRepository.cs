﻿using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    
    private readonly List<Post> posts = new List<Post>();
    public PostInMemoryRepository()
    {
        // Dummy data
        posts.Add(new Post { Id = 1, Title = "Hello World", Body = "Mit første post!", UserId = 1 });
        posts.Add(new Post { Id = 2, Title = "Nyheder", Body = "I dag er det solskin.", UserId = 2 });
        posts.Add(new Post { Id = 3, Title = "Spørgsmål", Body = "Hvordan virker async i C#?", UserId = 1 });
    }
    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any()
            ? posts.Max(p => p.Id) + 1
            : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }
    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }
    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }
    public Task<Post> GetSingleAsync(int id)
    {
        var post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
            throw new InvalidOperationException($"Post with ID '{id}' not found");

        return Task.FromResult(post);
    }
    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }

}