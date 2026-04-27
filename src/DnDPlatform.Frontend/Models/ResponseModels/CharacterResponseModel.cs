
using System.Text.Json.Serialization;

namespace DnDPlatform.Frontend.Models.ResponseModels;

public class CharacterResponseModel
{
    public Guid id { get; set; }
    public string name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string backstory { get; set; } = string.Empty;
    public int level { get; set; }
    public Guid templateId { get; set; }
    public string templateName { get; set; } = string.Empty;
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public SheetModel? currentSheet { get; set; }
}   