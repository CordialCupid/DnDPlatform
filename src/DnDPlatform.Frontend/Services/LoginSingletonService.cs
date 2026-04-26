using DnDPlatform.Frontend.Models.ReponseModels;

namespace DnDPlatform.Frontend.Services;
public class LoginSingletonService
{
    public bool LoggedIn {get;set;} = false;

    public MeStructure? MeAccount {get;set;}
}