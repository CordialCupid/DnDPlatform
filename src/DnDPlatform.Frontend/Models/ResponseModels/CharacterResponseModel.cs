namespace DnDPlatform.Frontend.Models.ReponseModels;

public class CharacterResponseModel
{
    public string Id {get;set;} = String.Empty;
    public string Name {get;set;} = String.Empty;
    public string Class {get;set;} = String.Empty;
    public string BackStory {get;set;} = String.Empty;
    public int Level {get;set;}
    public string TemplateId {get;set;} = String.Empty;
    public string TemplateName {get;set;} = String.Empty;
}   