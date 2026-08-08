using PersonalDigitalVault.Api.Entities;

namespace PersonalDigitalVault.Api.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<List<User>> GetAllAsync();

    Task<int> CountAsync();

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task SaveChangesAsync();
}