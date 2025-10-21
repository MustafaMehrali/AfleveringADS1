namespace ApiContracts.DTOs;

public class UpdateUserDto
{
    public int Id { get; set; }
    public required string UserName { get; set; }
}