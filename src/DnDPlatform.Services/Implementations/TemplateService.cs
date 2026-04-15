using DnDPlatform.Models;
using DnDPlatform.Models.Domain;
using DnDPlatform.Models.DTOs.Templates;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Algorithms;
using DnDPlatform.Services.Interfaces;
using System.Text.Json;

namespace DnDPlatform.Services.Implementations;

public class TemplateService(ITemplateRepository repo) : ITemplateService
{
    public async Task<IEnumerable<TemplateDto>> GetTemplatesAsync(string? system = null)
    {
        var templates = await repo.GetAllAsync(system);
        return templates.Select(MapToDto);
    }

    public async Task<TemplateDto> GetTemplateAsync(Guid id)
    {
        var template = await repo.GetByIdAsync(id, includeFields: true)
            ?? throw new KeyNotFoundException($"Template {id} not found.");
        return MapToDto(template);
    }

    public async Task<TemplateDto> CreateTemplateAsync(Guid userId, CreateTemplateRequest request)
    {
        // Validate JsonSchema is well-formed JSON
        try { JsonDocument.Parse(request.JsonSchema).Dispose(); }
        catch { throw new ArgumentException("JsonSchema is not valid JSON."); }

        try { JsonDocument.Parse(request.DefaultLayoutJson).Dispose(); }
        catch { throw new ArgumentException("DefaultLayoutJson is not valid JSON."); }

        var template = new Template
        {
            OwnerId = userId,
            Name = request.Name,
            System = request.System,
            Type = request.Type,
            JsonSchema = request.JsonSchema,
            DefaultLayoutJson = request.DefaultLayoutJson,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await repo.InsertAsync(template);
        return MapToDto(saved);
    }

    public async Task<SheetValidationResult> ValidateSheetAsync(Guid templateId, string sheetBlob)
    {
        var template = await repo.GetByIdAsync(templateId)
            ?? throw new KeyNotFoundException($"Template {templateId} not found.");
        return SheetValidationEngine.Validate(template, sheetBlob);
    }

    private static TemplateDto MapToDto(Template t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        System = t.System,
        Type = t.Type,
        JsonSchema = t.JsonSchema,
        DefaultLayoutJson = t.DefaultLayoutJson,
        OwnerId = t.OwnerId,
        CreatedAt = t.CreatedAt
    };
}
