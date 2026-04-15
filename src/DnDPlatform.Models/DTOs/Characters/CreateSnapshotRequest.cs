using System.ComponentModel.DataAnnotations;

namespace DnDPlatform.Models.DTOs.Characters;

public class CreateSnapshotRequest
{
    [Required, MinLength(1), MaxLength(100)]
    public string Label { get; set; } = string.Empty;
}
