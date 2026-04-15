using DnDPlatform.Models.DTOs.Characters;

namespace DnDPlatform.Services.Interfaces;

public interface ICharacterService
{
    Task<IEnumerable<CharacterDto>> GetCharactersAsync(Guid userId);
    Task<CharacterDto> GetCharacterAsync(Guid userId, Guid characterId);
    Task DeleteCharacterAsync(Guid userId, Guid characterId);
    Task<byte[]> ExportPdfAsync(Guid userId, Guid characterId);
}
