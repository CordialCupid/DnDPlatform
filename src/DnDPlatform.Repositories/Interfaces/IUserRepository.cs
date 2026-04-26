using DnDPlatform.Models.Domain;

namespace DnDPlatform.Repositories.Interfaces;

public interface IUserRepository
{
    //Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    //Task<User?> GetByEmailAsync(string email);
    Task<User> InsertAsync(User user);
    Task<bool> ExistsAsync(string username, string email);
}
