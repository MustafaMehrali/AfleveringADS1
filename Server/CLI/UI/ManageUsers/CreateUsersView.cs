using RepositoryContracts;
using Entities;

namespace CLI.UI.ManageUsers;

public class CreateUsersView
{
    private readonly IUserRepository userRepository;

    public CreateUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    private async Task CreateUsersAsync(string name, string password)
    {
        User user = new User
        {
            Username = name,
            Password = password,
        };
        User created = await userRepository.AddAsync(user);
        Console.WriteLine($"User oprettet! ID: {created.Id}");
    }
}