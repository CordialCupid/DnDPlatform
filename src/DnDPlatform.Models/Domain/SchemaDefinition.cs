namespace DnDPlatform.Models.Domain;

public class SchemaDefinition
{
    public string name     { get; set; }
    public string label    { get; set; }
    public string type     { get; set; }  
    public bool   required { get; set; }
}