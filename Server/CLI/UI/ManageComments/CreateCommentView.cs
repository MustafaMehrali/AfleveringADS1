using RepositoryContracts;
using Entities;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CreateCommentView(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    private async Task<Comment> AddCommentToPostAsync(string body, int userId, int postId)
    {
        // 1) Sikr at posten findes (GetSingleAsync kaster hvis ikke)
        Post post = await postRepository.GetSingleAsync(postId);

        // 2) Lav kommentar-objektet
        var comment = new Comment
        {
            Body = body,
            UserId = userId,
            PostId = post.Id
        };

        Comment created = await commentRepository.AddAsync(comment);
        return created;
    }
}

   