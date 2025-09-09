namespace CLI.UI.ManagePosts;
using RepositoryContracts;
using Entities;

public class ListPostView
{
    
    private readonly IPostRepository postRepository;

    public ListPostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    private async Task ShowPostsAsync()
    {
        var posts = postRepository.GetMany().ToList();

        if (posts.Count == 0)
        {
            Console.WriteLine("Ingen posts fundet.");
            return;
        }

        Console.WriteLine("Posts overview:");
        foreach (var post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }

        await Task.CompletedTask; // bare for at metoden er async
    }
}