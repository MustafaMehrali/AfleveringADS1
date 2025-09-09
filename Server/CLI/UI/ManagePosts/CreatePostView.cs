using System.Reflection;
using RepositoryContracts;
using Entities;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;

    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    private async Task AddPostAsync(string title, string body, int userId)
    {
        // Lav et Post-objekt ud fra parametrene
        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        // Tilføj posten via repository
        Post created = await postRepository.AddAsync(post);

        Console.WriteLine($"Post oprettet! ID: {created.Id}");
    }
}