using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfCharacterSheetRepository : ICharacterSheetRepository
{
    private readonly DnDDbContext _db;
    public EfCharacterSheetRepository(DnDDbContext db)
    {
        _db = db;
    }
    public Task<CharacterSheet?> GetCurrentAsync(Guid characterId)
    {
        return _db.CharacterSheets.Where(s => s.CharacterId == characterId).OrderByDescending(s => s.VersionNumber).FirstOrDefaultAsync();
    }

    public Task<CharacterSheet?> GetByVersionAsync(Guid characterId, int versionNumber)
    {
        return _db.CharacterSheets.FirstOrDefaultAsync(s => s.CharacterId == characterId && s.VersionNumber == versionNumber);
    }

    public Task<IEnumerable<CharacterSheet>> GetVersionHistoryAsync(Guid characterId)
    {
        return Task.FromResult<IEnumerable<CharacterSheet>>(_db.CharacterSheets.Where(s => s.CharacterId == characterId).OrderByDescending(s => s.VersionNumber).AsEnumerable()); 
    }

    public async Task<CharacterSheet> InsertAsync(CharacterSheet sheet)
    {
        _db.CharacterSheets.Add(sheet);
        await _db.SaveChangesAsync();
        return sheet;
    }

    public async Task<int> GetNextVersionNumberAsync(Guid characterId)
    {

        var max = await _db.CharacterSheets.Where(s => s.CharacterId == characterId).MaxAsync(s => (int?)s.VersionNumber);

        // if max return as null, set it to 0. Always increment
        return (max ?? 0) + 1;
    }
}
