using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfUserRepository(DnDDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByUsernameAsync(string username) =>
        db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<User?> GetByEmailAsync(string email) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> InsertAsync(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public Task<bool> ExistsAsync(string username, string email) =>
        db.Users.AnyAsync(u => u.Username == username || u.Email == email);
}
