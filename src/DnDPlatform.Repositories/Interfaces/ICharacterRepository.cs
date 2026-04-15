using DnDPlatform.Models.Domain;

namespace DnDPlatform.Repositories.Interfaces;

public interface ICharacterRepository
{
    Task<IEnumerable<Character>> GetAllByOwnerAsync(Guid ownerId);
    Task<Character?> GetByIdAsync(Guid id, bool includeSheets = false);
    Task<Character> InsertAsync(Character character);
    Task<Character> UpdateAsync(Character character);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id, Guid ownerId);
}
