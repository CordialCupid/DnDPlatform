using DnDPlatform.Models.Domain;

namespace DnDPlatform.Repositories.Interfaces;

public interface ICharacterSheetRepository
{
    Task<CharacterSheet?> GetCurrentAsync(Guid characterId);
    Task<CharacterSheet?> GetByVersionAsync(Guid characterId, int versionNumber);
    Task<IEnumerable<CharacterSheet>> GetVersionHistoryAsync(Guid characterId);
    Task<CharacterSheet> InsertAsync(CharacterSheet sheet);
    Task<int> GetNextVersionNumberAsync(Guid characterId);
}
