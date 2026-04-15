using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Models.DTOs.Characters;

public class SaveSheetRequest
{
    [Required]
    public string SheetData { get; set; } = "{}";
}
