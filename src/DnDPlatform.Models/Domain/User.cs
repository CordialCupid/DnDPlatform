namespace DnDPlatform.Models.Domain;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Character> Characters { get; set; } = [];
    public ICollection<Template> Templates { get; set; } = [];
}
