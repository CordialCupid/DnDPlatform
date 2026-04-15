namespace DnDPlatform.Models.Domain;

public class FieldDefinition
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public Template Template { get; set; } = null!;
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public string ValidationRulesJson { get; set; } = "{}";
    public string? DefaultValue { get; set; }
    public int DisplayOrder { get; set; }
}
