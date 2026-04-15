using DnDPlatform.Models.DTOs.Characters;

namespace DnDPlatform.Services.Interfaces;

public interface IVersionSnapshotManager
{
    Task<SheetVersionDto> InitializeAsync(Guid characterId, string initialBlob);
    Task<SheetVersionDto> SaveCurrentAsync(Guid userId, Guid characterId, string blob);
    Task<SheetVersionDto> CreateSnapshotAsync(Guid userId, Guid characterId, string blob, string label);
    Task<IEnumerable<VersionSummaryDto>> GetVersionHistoryAsync(Guid userId, Guid characterId);
    Task<SheetVersionDto> RevertAsync(Guid userId, Guid characterId, int targetVersion);
}
