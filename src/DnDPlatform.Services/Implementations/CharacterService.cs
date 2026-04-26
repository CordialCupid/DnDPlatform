using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Algorithms;
using DnDPlatform.Services.Events;
using DnDPlatform.Services.Interfaces;
using DnDPlatform.Models.Domain;
using System.Text;

namespace DnDPlatform.Services.Implementations;

public class CharacterService : ICharacterService
{

    private readonly ICharacterRepository _characterRepo;
    private readonly ITemplateRepository _templateRepo;
    private readonly ICharacterSheetRepository _sheetRepo;
    private readonly IEventBus _eventBus;

    public CharacterService(ICharacterRepository characterRepo, ITemplateRepository templateRepo, ICharacterSheetRepository sheetRepo ,IEventBus eventBus)
    {
        _characterRepo = characterRepo;
        _templateRepo = templateRepo;
        _sheetRepo = sheetRepo;
        _eventBus = eventBus;
    }
    public async Task<IEnumerable<CharacterDto>> GetCharactersAsync(Guid userId)
    {
        var characters = await _characterRepo.GetAllByOwnerAsync(userId);
        var dtos = new List<CharacterDto>();

        foreach (var c in characters)
        {
            var sheet = await _sheetRepo.GetCurrentAsync(c.Id);
            dtos.Add(MapToDto(c, sheet));
        }

        return dtos;
    }

    public async Task<CharacterDto> GetCharacterAsync(Guid userId, Guid characterId)
    {
        var character = await _characterRepo.GetByIdAsync(characterId);
        if (character == null)
        {
            throw new KeyNotFoundException($"Character {characterId} not found.");
        }

        if (character.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Access has been denied ");         
        }

        var sheet = await _sheetRepo.GetCurrentAsync(characterId);

        if (sheet is not null)
        {
            var template = await _templateRepo.GetByIdAsync(character.TemplateId);
            if (template is not null)
            {
                sheet.JsonBlob = CalculatedFieldEvaluator.Evaluate(template.JsonSchema, sheet.JsonBlob);            
            }
        }

        return MapToDto(character, sheet);
    }

    public async Task DeleteCharacterAsync(Guid userId, Guid characterId)
    {
        var character = await _characterRepo.GetByIdAsync(characterId);
        
        if (character == null)
        {
            throw new KeyNotFoundException($"Character {characterId} not found.");
        }
        if (character.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Access denied.");
            
        }

        await _characterRepo.DeleteAsync(characterId);

        await _eventBus.PublishAsync(new CharacterDeletedEvent
        {
            UserId = userId,
            CharacterId = characterId,
            CharacterName = character.Name
        });
    }

    private static CharacterDto MapToDto(Character c, CharacterSheet? sheet) => new()
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
