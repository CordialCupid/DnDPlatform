using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Frontend.Models.FormModels;

public class SheetFormModel
{
    [Required]
    public string sheetData {get;set;} = "{}";
}