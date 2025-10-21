namespace ApiContracts.DTOs;

public class UpdatePostDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}