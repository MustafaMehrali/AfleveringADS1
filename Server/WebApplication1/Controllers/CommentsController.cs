using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")] // Changed from [controller] to api/[controller]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _comments;

    public CommentsController(ICommentRepository comments)
    {
        _comments = comments;
    }

    // Create
    [HttpPost]
    public async Task<ActionResult<Comment>> Create([FromBody] Comment comment)
    {
        var created = await _comments.AddAsync(comment);
        return CreatedAtAction(nameof(GetSingle), new { id = created.Id }, created);
    }

    // Update
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Comment comment)
    {
        if (id != comment.Id) return BadRequest("Route id and body id must match.");
        await _comments.UpdateAsync(comment);
        return NoContent();
    }

    // GetSingle
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Comment>> GetSingle(int id)
    {
        var comment = await _comments.GetSingleAsync(id);
        return Ok(comment);
    }

    // GetMany: filter by postId and/or userId, plus search in body
    [HttpGet]
    public ActionResult<IEnumerable<Comment>> GetMany(
        [FromQuery] int? postId,
        [FromQuery] int? userId,
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100)
    {
        if (take is < 1 or > 500) return BadRequest("take must be between 1 and 500.");

        var query = _comments.GetMany();

        if (postId.HasValue) query = query.Where(c => c.PostId == postId.Value);
        if (userId.HasValue) query = query.Where(c => c.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(c => c.Body.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        var items = query
            .OrderByDescending(c => c.Id)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Ok(items);
    }

    // Delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _comments.DeleteAsync(id);
        return NoContent();
    }
}