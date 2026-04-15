using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Algorithms;
using DnDPlatform.Services.Events;
using DnDPlatform.Services.Interfaces;
using System.Text;

namespace DnDPlatform.Services.Implementations;

public class CharacterService(
    ICharacterRepository characterRepo,
    ICharacterSheetRepository sheetRepo,
    ITemplateRepository templateRepo,
    IAuditLogService auditLog,
    IEventBus eventBus) : ICharacterService
{
    public async Task<IEnumerable<CharacterDto>> GetCharactersAsync(Guid userId)
    {
        var characters = await characterRepo.GetAllByOwnerAsync(userId);
        var dtos = new List<CharacterDto>();

        foreach (var c in characters)
        {
            var sheet = await sheetRepo.GetCurrentAsync(c.Id);
            dtos.Add(MapToDto(c, sheet));
        }

        return dtos;
    }

    public async Task<CharacterDto> GetCharacterAsync(Guid userId, Guid characterId)
    {
        var character = await characterRepo.GetByIdAsync(characterId)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");

        if (character.OwnerId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        var sheet = await sheetRepo.GetCurrentAsync(characterId);

        // Apply calculated fields
        if (sheet is not null)
        {
            var template = await templateRepo.GetByIdAsync(character.TemplateId);
            if (template is not null)
                sheet.JsonBlob = CalculatedFieldEvaluator.Evaluate(template.JsonSchema, sheet.JsonBlob);
        }

        return MapToDto(character, sheet);
    }

    public async Task DeleteCharacterAsync(Guid userId, Guid characterId)
    {
        var character = await characterRepo.GetByIdAsync(characterId)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");

        if (character.OwnerId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        await characterRepo.DeleteAsync(characterId);

        await eventBus.PublishAsync(new CharacterDeletedEvent
        {
            UserId = userId,
            CharacterId = characterId,
            CharacterName = character.Name
        });
    }

    public async Task<byte[]> ExportPdfAsync(Guid userId, Guid characterId)
    {
        var character = await characterRepo.GetByIdAsync(characterId)
            ?? throw new KeyNotFoundException($"Character {characterId} not found.");

        if (character.OwnerId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        var sheet = await sheetRepo.GetCurrentAsync(characterId);

        // Minimal plain-text PDF export (no external PDF library dependency)
        var sb = new StringBuilder();
        sb.AppendLine($"Character Sheet: {character.Name}");
        sb.AppendLine($"Class: {character.Class}  |  Level: {character.Level}");
        sb.AppendLine($"Template: {character.Template?.Name}");
        sb.AppendLine($"Backstory: {character.Backstory}");
        sb.AppendLine();
        sb.AppendLine("--- Sheet Data ---");
        sb.AppendLine(sheet?.JsonBlob ?? "{}");
        sb.AppendLine($"Exported at: {DateTime.UtcNow:u}");

        await auditLog.LogAsync(userId, userId.ToString(), AuditAction.Export, ResourceType.Character, characterId);

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static CharacterDto MapToDto(Models.Domain.Character c, Models.Domain.CharacterSheet? sheet) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Class = c.Class,
        Backstory = c.Backstory,
        Level = c.Level,
        TemplateId = c.TemplateId,
        TemplateName = c.Template?.Name ?? string.Empty,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        CurrentSheet = sheet is null ? null : new SheetVersionDto
        {
            Id = sheet.Id,
            CharacterId = sheet.CharacterId,
            VersionNumber = sheet.VersionNumber,
            Label = sheet.Label,
            JsonBlob = sheet.JsonBlob,
            IsSnapshot = sheet.IsSnapshot,
            CreatedAt = sheet.CreatedAt
        }
    };
}
