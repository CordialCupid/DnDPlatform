using DnDPlatform.Frontend.Models.ResponseModels;

namespace DnDPlatform.Frontend.Services;
public static class LoginSingletonService
{
    public static bool LoggedIn {get;set;} = false;

    public static MeStructure? MeAccount {get;set;}
}