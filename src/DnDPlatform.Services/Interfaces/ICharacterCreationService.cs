using DnDPlatform.Models.DTOs.Characters;

namespace DnDPlatform.Services.Interfaces;

public interface ICharacterCreationService
{
    Task<CharacterDto> CreateCharacterAsync(Guid userId, CreateCharacterRequest request);
}
