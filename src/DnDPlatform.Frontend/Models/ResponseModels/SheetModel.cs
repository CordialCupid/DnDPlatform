namespace DnDPlatform.Frontend.Models.ResponseModels;
public class SheetModel
{
    public Guid id { get; set; }
    public Guid characterId { get; set; }
    public int versionNumber { get; set; }
    public string label { get; set; } = string.Empty;
    public string jsonBlob { get; set; } = "{}";
    public bool isSnapshot { get; set; }
    public DateTime createdAt { get; set; }
}