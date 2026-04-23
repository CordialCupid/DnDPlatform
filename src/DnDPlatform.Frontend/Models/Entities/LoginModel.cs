using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Frontend.Models.Entities;
public class LoginModel
{
    [Required]
    public string Username {get;set;} = "";
    [Required]
    public string Password {get;set;} = "";
}