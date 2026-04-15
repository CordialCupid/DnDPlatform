namespace DnDPlatform.Models;

public class SheetValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; set; } = [];
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
