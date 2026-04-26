using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfUserRepository : IUserRepository
{
    private readonly DnDDbContext _db;
    public EfUserRepository(DnDDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return _db.Users.FirstOrDefaultAsync(u => u.Username == username); 
    }

    public async Task<User> InsertAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public Task<bool> ExistsAsync(string username, string email)
    {
        return _db.Users.AnyAsync(u => u.Username == username || u.Email == email);
    }
}
