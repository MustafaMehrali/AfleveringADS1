namespace ApiContracts.DTOs;

public class UpdateCommentDto
{
    public int Id { get; set; }
    public required string Body { get; set; }
}