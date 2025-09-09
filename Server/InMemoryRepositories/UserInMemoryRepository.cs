using Entities;
using RepositoryContracts;
namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new List<User>();
    public UserInMemoryRepository()
    {
        // Dummy data
        users.Add(new User { Id = 1, Username = "Alice", Password = "1234" });
        users.Add(new User { Id = 2, Username = "Bob", Password = "qwerty" });
        users.Add(new User { Id = 3, Username = "Charlie", Password = "password" });
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        var existing = users.SingleOrDefault(u => u.Id == user.Id);
        if (existing is null)
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");

        users.Remove(existing);
        users.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var toRemove = users.SingleOrDefault(u => u.Id == id);
        if (toRemove is null)
            throw new InvalidOperationException($"User with ID '{id}' not found");

        users.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        var user = users.SingleOrDefault(u => u.Id == id);
        if (user is null)
            throw new InvalidOperationException($"User with ID '{id}' not found");

        return Task.FromResult(user);
    }

    public IQueryable<User> GetMany() => users.AsQueryable();
}

