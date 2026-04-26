using DnDPlatform.Models.Domain;
using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Events;
using DnDPlatform.Services.Interfaces;

namespace DnDPlatform.Services.Implementations;

public class VersionSnapshotManager : IVersionSnapshotManager
{
    private readonly ICharacterSheetRepository _sheetRepo;
    private readonly ICharacterRepository _characterRepo;
    private readonly IAuditLogService _auditLog;

    private readonly IEventBus _eventBus;

    public VersionSnapshotManager(ICharacterSheetRepository sheetRepo, ICharacterRepository characterRepo, IAuditLogService auditLog, IEventBus eventBus)
    {
        _sheetRepo = sheetRepo;
        _characterRepo = characterRepo;
        _auditLog = auditLog;
        _eventBus = eventBus;
    }

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
        var saved = await _sheetRepo.InsertAsync(sheet);
        return MapToDto(saved);
    }

    public async Task<SheetVersionDto> SaveCurrentAsync(Guid userId, Guid characterId, string blob)
    {
        await EnsureOwnershipAsync(userId, characterId);

        var nextVersion = await _sheetRepo.GetNextVersionNumberAsync(characterId);
        var sheet = new CharacterSheet
        {
            CharacterId = characterId,
            VersionNumber = nextVersion,
            Label = $"Save v{nextVersion}",
            JsonBlob = blob,
            IsSnapshot = false,
            CreatedAt = DateTime.UtcNow
        };
        var saved = await _sheetRepo.InsertAsync(sheet);

        var character = await _characterRepo.GetByIdAsync(characterId);
        if (character is not null)
        {
            character.UpdatedAt = DateTime.UtcNow;
            await _characterRepo.UpdateAsync(character);
        }

        await _eventBus.PublishAsync(new CharacterSheetSavedEvent
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

        var nextVersion = await _sheetRepo.GetNextVersionNumberAsync(characterId);
        var sheet = new CharacterSheet
        {
            CharacterId = characterId,
            VersionNumber = nextVersion,
            Label = label,
            JsonBlob = blob,
            IsSnapshot = true,
            CreatedAt = DateTime.UtcNow
        };
        var saved = await _sheetRepo.InsertAsync(sheet);

        await _eventBus.PublishAsync(new CharacterSheetSavedEvent
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
        var sheets = await _sheetRepo.GetVersionHistoryAsync(characterId);
        return sheets.Select(s => new VersionSummaryDto
        {
            Id = s.Id,
            VersionNumber = s.VersionNumber,
            Label = s.Label,
            IsSnapshot = s.IsSnapshot,
            CreatedAt = s.CreatedAt
        });
    }

    // method to ensure the passed in user id actually owns the pased in character id character
    private async Task EnsureOwnershipAsync(Guid userId, Guid characterId)
    {
        var character = await _characterRepo.GetByIdAsync(characterId);

        if (character == null)
        {
            throw new KeyNotFoundException($"Character {characterId} not found.");
        }
        if (character.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Access has been denied.");
        }
    }

    private static SheetVersionDto MapToDto(CharacterSheet s)
    {
        return new()
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
}
