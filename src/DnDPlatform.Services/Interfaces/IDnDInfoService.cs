using DnDPlatform.Models.DTOs.DnD5e;

namespace DnDPlatform.Services.Interfaces;

public interface IDnDInfoService
{
    Task<IEnumerable<DnDClassDto>> GetClassesAsync();
    Task<IEnumerable<DnDSpellDto>> GetSpellsAsync(string? filter = null);
    Task<IEnumerable<DnDEquipmentDto>> GetEquipmentAsync();
    Task<IEnumerable<DnDAbilityScoreDto>> GetAbilityScoresAsync();
}
