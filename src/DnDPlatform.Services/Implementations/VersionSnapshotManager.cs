using DnDPlatform.Models.Domain;
using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Events;
using DnDPlatform.Services.Interfaces;

namespace DnDPlatform.Services.Implementations;

public class VersionSnapshotManager(
    ICharacterSheetRepository sheetRepo,
    ICharacterRepository characterRepo,
    IAuditLogService auditLog,
    IEventBus eventBus) : IVersionSnapshotManager
{
    public async Task<SheetVersionDto> InitializeAsync(Guid characterId, string initialBlob)
    {
        var sheet = new CharacterSheet
        {
            CharacterId = characterId,
            VersionNumber = 1,
            Label = "Initial",
            JsonBlob = initialBlob,
            IsSnapshot = false,
            CreatedAt = DateTime.UtcNow
        };
        var saved = await sheetRepo.InsertAsync(sheet);
        return MapToDto(saved);
    }

    public async Task<SheetVersionDto> SaveCurrentAsync(Guid userId, Guid characterId, string blob)
    {
        await EnsureOwnershipAsync(userId, characterId);

        var nextVersion = await sheetRepo.GetNextVersionNumberAsync(characterId);
        var sheet = new CharacterSheet
        {
            CharacterId = characterId,
            VersionNumber = nextVersion,
            Label = $"Save v{nextVersion}",
            JsonBlob = blob,
            IsSnapshot = false,
            CreatedAt = DateTime.UtcNow
        };
        var saved = await sheetRepo.InsertAsync(sheet);

        var character = await characterRepo.GetByIdAsync(characterId);
        if (character is not null)
        {
            character.UpdatedAt = DateTime.UtcNow;
            await characterRepo.UpdateAsync(character);
        }

        await eventBus.PublishAsync(new CharacterSheetSavedEvent
        {
            UserId = userId,
            CharacterId = characterId,
            SheetId = saved.Id,
            VersionNumber = saved.VersionNumber,
            IsSnapshot = false
        });

        return MapToDto(saved);
    }

    public async Task<SheetVersionDto> CreateSnapshotAsync(Guid userId, Guid characterId, string blob, string label)
    {
        await EnsureOwnershipAsync(userId, characterId);

        var nextVersion = await sheetRepo.GetNextVersionNumberAsync(characterId);
        var sheet = new CharacterSheet
        {
            CharacterId = characterId,
            VersionNumber = nextVersion,
            Label = label,
            JsonBlob = blob,
            IsSnapshot = true,
            CreatedAt = DateTime.UtcNow
        };
        var saved = await sheetRepo.InsertAsync(sheet);

        await eventBus.PublishAsync(new CharacterSheetSavedEvent
        {
            UserId = userId,
            CharacterId = characterId,
            SheetId = saved.Id,
            VersionNumber = saved.VersionNumber,
            IsSnapshot = true
        });

        return MapToDto(saved);
    }

    public async Task<IEnumerable<VersionSummaryDto>> GetVersionHistoryAsync(Guid userId, Guid characterId)
    {
        await EnsureOwnershipAsync(userId, characterId);
        var sheets = await sheetRepo.GetVersionHistoryAsync(characterId);
        return sheets.Select(s => new VersionSummaryDto
        {
            Id = s.Id,
            VersionNumber = s.VersionNumber,
            Label = s.Label,
            IsSnapshot = s.IsSnapshot,
            CreatedAt = s.CreatedAt
        });
    }

    public async Task<SheetVersionDto> RevertAsync(Guid userId, Guid characterId, int targetVersion)
    {
        await EnsureOwnershipAsync(userId, characterId);

        var target = await sheetRepo.GetByVersionAsync(characterId, targetVersion)
            ?? throw new KeyNotFoundException($"Version {targetVersion} not found for character {characterId}.");

        var nextVersion = await sheetRepo.GetNextVersionNumberAsync(characterId);
        var reverted = new CharacterSheet
        {
            CharacterId = characterId,
            VersionNumber = nextVersion,
            Label = $"Reverted to v{targetVersion}",
            JsonBlob = target.JsonBlob,
            IsSnapshot = false,
            CreatedAt = DateTime.UtcNow
        };
        var saved = await sheetRepo.InsertAsync(reverted);

        await eventBus.PublishAsync(new CharacterSheetSavedEvent
        {
            UserId = userId,
            CharacterId = characterId,
            SheetId = saved.Id,
            VersionNumber = saved.VersionNumber,
            IsSnapshot = false
        });

        return MapToDto(saved);
    }

    private async Task EnsureOwnershipAsync(Guid userId, Guid characterId)
    {
        var character = await characterRepo.GetByIdAsync(characterId)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");
        if (character.OwnerId != userId)
            throw new UnauthorizedAccessException("Access denied.");
    }

    private static SheetVersionDto MapToDto(CharacterSheet s) => new()
    {
        Id = s.Id,
        CharacterId = s.CharacterId,
        VersionNumber = s.VersionNumber,
        Label = s.Label,
        JsonBlob = s.JsonBlob,
        IsSnapshot = s.IsSnapshot,
        CreatedAt = s.CreatedAt
    };
}
