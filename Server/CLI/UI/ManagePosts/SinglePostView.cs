namespace CLI.UI.ManagePosts;
using RepositoryContracts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public SinglePostView(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task ViewSinglePost(int id)
    {

        var posts = await postRepository.GetSingleAsync(id);
        
        Console.WriteLine($"[{posts.Id}] {posts.Title}");
        Console.WriteLine(posts.Body);
        
        var comments = commentRepository
            .GetMany()
            .Where(c => c.PostId == posts.Id)
            .ToList();

        Console.WriteLine("Kommentarer:");
        foreach (var c in comments)
        {
            Console.WriteLine($"- ({c.Id}) {c.Body} (User {c.UserId})");
        }
    }
}
