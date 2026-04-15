using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfCharacterSheetRepository(DnDDbContext db) : ICharacterSheetRepository
{
    public Task<CharacterSheet?> GetCurrentAsync(Guid characterId) =>
        db.CharacterSheets
            .Where(s => s.CharacterId == characterId)
            .OrderByDescending(s => s.VersionNumber)
            .FirstOrDefaultAsync();

    public Task<CharacterSheet?> GetByVersionAsync(Guid characterId, int versionNumber) =>
        db.CharacterSheets.FirstOrDefaultAsync(s => s.CharacterId == characterId && s.VersionNumber == versionNumber);

    public Task<IEnumerable<CharacterSheet>> GetVersionHistoryAsync(Guid characterId) =>
        Task.FromResult<IEnumerable<CharacterSheet>>(
            db.CharacterSheets
                .Where(s => s.CharacterId == characterId)
                .OrderByDescending(s => s.VersionNumber)
                .AsEnumerable());

    public async Task<CharacterSheet> InsertAsync(CharacterSheet sheet)
    {
        db.CharacterSheets.Add(sheet);
        await db.SaveChangesAsync();
        return sheet;
    }

    public async Task<int> GetNextVersionNumberAsync(Guid characterId)
    {
        var max = await db.CharacterSheets
            .Where(s => s.CharacterId == characterId)
            .MaxAsync(s => (int?)s.VersionNumber);
        return (max ?? 0) + 1;
    }
}
