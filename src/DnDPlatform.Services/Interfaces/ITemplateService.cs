using DnDPlatform.Models;
using DnDPlatform.Models.DTOs.Templates;

namespace DnDPlatform.Services.Interfaces;

public interface ITemplateService
{
    Task<IEnumerable<TemplateDto>> GetTemplatesAsync(string? system = null);
    Task<TemplateDto> GetTemplateAsync(Guid id);
    Task<TemplateDto> CreateTemplateAsync(Guid userId, CreateTemplateRequest request);
    Task<SheetValidationResult> ValidateSheetAsync(Guid templateId, string sheetBlob);
}
