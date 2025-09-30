using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;
    private readonly IUserRepository _users;

    // Inline response shape (move to ApiContracts later if you want)
    public record PostResponse(Post Post, string? AuthorUsername, IEnumerable<Comment>? Comments);

    public PostsController(IPostRepository posts, ICommentRepository comments, IUserRepository users)
    {
        _posts = posts;        // :contentReference[oaicite:1]{index=1}
        _comments = comments;  // :contentReference[oaicite:2]{index=2}
        _users = users;        // :contentReference[oaicite:3]{index=3}
    }

    // Create
    [HttpPost]
    public async Task<ActionResult<Post>> Create([FromBody] Post post)
    {
        var created = await _posts.AddAsync(post);
        return CreatedAtAction(nameof(GetSingle), new { id = created.Id }, created);
    }

    // Update
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Post post)
    {
        if (id != post.Id) return BadRequest("Route id and body id must match.");
        await _posts.UpdateAsync(post);
        return NoContent();
    }

    // GetSingle (with optional includes)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetSingle(
        int id,
        [FromQuery] bool includeComments = false,
        [FromQuery] bool includeAuthor = false)
    {
        var post = await _posts.GetSingleAsync(id);

        if (!includeComments && !includeAuthor) return Ok(post);

        string? username = null;
        IEnumerable<Comment>? comments = null;

        if (includeAuthor)
        {
            var user = await _users.GetSingleAsync(post.UserId);
            username = user.Username;
        }

        if (includeComments)
        {
            comments = _comments.GetMany().Where(c => c.PostId == post.Id).ToList();
        }

        return Ok(new PostResponse(post, username, comments));
    }

    // GetMany (filters + pagination + includes)
    // Examples:
    //   GET /api/posts?userId=2
    //   GET /api/posts?search=hello&skip=0&take=20&includeAuthor=true
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetMany(
        [FromQuery] int? userId,
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        [FromQuery] bool includeComments = false,
        [FromQuery] bool includeAuthor = false)
    {
        if (take is < 1 or > 200) return BadRequest("take must be between 1 and 200.");

        var query = _posts.GetMany(); // IQueryable<Post> :contentReference[oaicite:4]{index=4}

        if (userId.HasValue)
            query = query.Where(p => p.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(p => p.Title.Contains(s, StringComparison.OrdinalIgnoreCase)
                                   || p.Body.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        var items = query
            .OrderByDescending(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToList();

        if (!includeAuthor && !includeComments)
            return Ok(items);

        // Build enriched responses
        var usersById = includeAuthor
            ? _users.GetMany().ToDictionary(u => u.Id, u => u.Username)
            : new Dictionary<int, string>();
        var comments = includeComments ? _comments.GetMany().ToList() : new List<Comment>();

        var result = items.Select(p =>
            new PostResponse(
                p,
                includeAuthor && usersById.TryGetValue(p.UserId, out var name) ? name : null,
                includeComments ? comments.Where(c => c.PostId == p.Id) : null
            ));

        return Ok(result);
    }

    // Delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _posts.DeleteAsync(id);
        return NoContent();
    }

    // Nested: /api/posts/{postId}/comments
    [HttpGet("{postId:int}/comments")]
    public ActionResult<IEnumerable<Comment>> GetCommentsForPost(int postId)
    {
        var postComments = _comments.GetMany().Where(c => c.PostId == postId).ToList();
        return Ok(postComments);
    }

    // Nested create comment under a post
    [HttpPost("{postId:int}/comments")]
    public async Task<ActionResult<Comment>> AddCommentToPost(int postId, [FromBody] Comment comment)
    {
        if (comment.PostId != 0 && comment.PostId != postId)
            return BadRequest("Comment.PostId must match the route postId.");
        comment.PostId = postId;
        var created = await _comments.AddAsync(comment);
        return CreatedAtAction(nameof(CommentsController.GetSingle), "Comments", new { id = created.Id }, created);
    }
}
