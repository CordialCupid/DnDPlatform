using DnDPlatform.Models.Domain;
using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Events;
using DnDPlatform.Services.Interfaces;

namespace DnDPlatform.Services.Implementations;

public class CharacterCreationService(
    ICharacterRepository characterRepo,
    ITemplateRepository templateRepo,
    IDnDInfoService dndInfoService,
    IVersionSnapshotManager versionManager,
    IAuditLogService auditLog,
    IEventBus eventBus) : ICharacterCreationService
{
    public async Task<CharacterDto> CreateCharacterAsync(Guid userId, CreateCharacterRequest request)
    {
        var template = await templateRepo.GetByIdAsync(request.TemplateId)
            ?? throw new KeyNotFoundException($"Template {request.TemplateId} not found.");

        // Pre-fetch D&D reference data (warms the cache)
        _ = dndInfoService.GetClassesAsync();

        var character = new Character
        {
            OwnerId = userId,
            TemplateId = request.TemplateId,
            Name = request.Name,
            Class = request.Class,
            Backstory = request.Backstory,
            Level = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var saved = await characterRepo.InsertAsync(character);

        // Write version 1 sheet blob seeded from template defaults
        await versionManager.InitializeAsync(saved.Id, template.JsonSchema);

        await eventBus.PublishAsync(new CharacterCreatedEvent
        {
            UserId = userId,
            CharacterId = saved.Id,
            CharacterName = saved.Name
        });

        return new CharacterDto
        {
            Id = saved.Id,
            Name = saved.Name,
            Class = saved.Class,
            Backstory = saved.Backstory,
            Level = saved.Level,
            TemplateId = saved.TemplateId,
            TemplateName = template.Name,
            CreatedAt = saved.CreatedAt,
            UpdatedAt = saved.UpdatedAt
        };
    }
}
