namespace DnDPlatform.Models.Domain;

public class Character
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Backstory { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public Guid TemplateId { get; set; }
    public Template Template { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<CharacterSheet> Sheets { get; set; } = [];
}
