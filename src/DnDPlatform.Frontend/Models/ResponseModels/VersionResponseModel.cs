namespace DnDPlatform.Frontend.Models.ResponseModels;

public class VersionResponseModel
{
    public Guid id {get;set;}
    public int versionNumber {get;set;}
    public string label {get;set;} = String.Empty;
    public bool isSnapshot {get;set;} 
    public DateTime createdAt {get;set;}
}