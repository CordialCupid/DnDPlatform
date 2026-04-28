namespace DnDPlatform.Models.Domain;

public class SchemaDefinition
{
    public string name     { get; set; } = String.Empty;
    public string label    { get; set; } = String.Empty;
    public string type     { get; set; } = String.Empty;
    public bool   required { get; set; } 
}