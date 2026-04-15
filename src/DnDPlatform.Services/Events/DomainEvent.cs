namespace DnDPlatform.Services.Events;

public abstract class DomainEvent
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

public class CharacterSheetSavedEvent : DomainEvent
{
    public Guid CharacterId { get; init; }
    public Guid SheetId { get; init; }
    public int VersionNumber { get; init; }
    public bool IsSnapshot { get; init; }
}

public class CharacterDeletedEvent : DomainEvent
{
    public Guid CharacterId { get; init; }
    public string CharacterName { get; init; } = string.Empty;
}

public class CharacterCreatedEvent : DomainEvent
{
    public Guid CharacterId { get; init; }
    public string CharacterName { get; init; } = string.Empty;
}
