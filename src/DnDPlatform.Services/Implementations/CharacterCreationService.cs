using DnDPlatform.Models.Domain;
using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Models.Enums;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Events;
using DnDPlatform.Services.Interfaces;

namespace DnDPlatform.Services.Implementations;

public class CharacterCreationService : ICharacterCreationService
{
    private readonly ICharacterRepository _characterRepo;
    private readonly ITemplateRepository _templateRepo;
    private readonly IVersionSnapshotManager _snapshotManager;
    private readonly IEventBus _eventBus;

    public CharacterCreationService(ICharacterRepository characterRepo, ITemplateRepository templateRepo, IVersionSnapshotManager snapshotManager,IEventBus eventBus)
    {
        _characterRepo = characterRepo;
        _templateRepo = templateRepo;
        _snapshotManager = snapshotManager;
        _eventBus = eventBus;
    }


    public async Task<CharacterDto> CreateCharacterAsync(Guid userId, CreateCharacterRequest request)
    {
        var template = await _templateRepo.GetByIdAsync(request.TemplateId);
        
        if (template == null)
        {
            throw new KeyNotFoundException($"Template {request.TemplateId} not found.");
        }

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

        var saved = await _characterRepo.InsertAsync(character);

        await _snapshotManager.InitializeAsync(saved.Id, template.JsonSchema);

        await _eventBus.PublishAsync(new CharacterCreatedEvent
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
