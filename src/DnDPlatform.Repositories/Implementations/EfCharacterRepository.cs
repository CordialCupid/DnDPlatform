using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfCharacterRepository(DnDDbContext db) : ICharacterRepository
{
    public Task<IEnumerable<Character>> GetAllByOwnerAsync(Guid ownerId) =>
        Task.FromResult<IEnumerable<Character>>(
            db.Characters.Include(c => c.Template).Where(c => c.OwnerId == ownerId).AsEnumerable());

    public Task<Character?> GetByIdAsync(Guid id, bool includeSheets = false)
    {
        IQueryable<Character> query = db.Characters.Include(c => c.Template);
        if (includeSheets)
            query = query.Include(c => c.Sheets);
        return query.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Character> InsertAsync(Character character)
    {
        db.Characters.Add(character);
        await db.SaveChangesAsync();
        return character;
    }

    public async Task<Character> UpdateAsync(Character character)
    {
        character.UpdatedAt = DateTime.UtcNow;
        db.Characters.Update(character);
        await db.SaveChangesAsync();
        return character;
    }

    public async Task DeleteAsync(Guid id)
    {
        var character = await db.Characters.FindAsync(id);
        if (character is not null)
        {
            db.Characters.Remove(character);
            await db.SaveChangesAsync();
        }
    }

    public Task<bool> ExistsAsync(Guid id, Guid ownerId) =>
        db.Characters.AnyAsync(c => c.Id == id && c.OwnerId == ownerId);
}
