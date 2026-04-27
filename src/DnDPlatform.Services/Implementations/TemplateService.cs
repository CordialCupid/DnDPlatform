using DnDPlatform.Models;
using DnDPlatform.Models.Domain;
using DnDPlatform.Models.DTOs.Templates;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Interfaces;
using System.Text.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;


namespace DnDPlatform.Services.Implementations;

public class TemplateService : ITemplateService
{
    private readonly ITemplateRepository _repo;

    public TemplateService(ITemplateRepository repo)
    {
        _repo = repo;
    }
    public async Task<IEnumerable<TemplateDto>> GetTemplatesAsync(string? system = null)
    {
        var templates = await _repo.GetAllAsync(system);
        return templates.Select(MapToDto);
    }

    public async Task<TemplateDto> GetTemplateAsync(Guid id)
    {
        var template = await _repo.GetByIdAsync(id, includeFields: true);
        if (template == null)
        {
            throw new Exception($"Template {id} not found!");
        }
        return MapToDto(template);
    }

    public async Task<TemplateDto> CreateTemplateAsync(Guid userId, CreateTemplateRequest request)
    {

        try 
        { 
            JsonDocument.Parse(request.JsonSchema).Dispose(); 
        }
        catch 
        { 
            throw new ArgumentException("JsonSchema is not valid JSON"); 
        }

        try 
        { 
            JsonDocument.Parse(request.DefaultLayoutJson).Dispose(); 
        }
        catch 
        { 
            throw new ArgumentException("DefaultLayoutJson is not valid JSON"); 
        }

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

        var saved = await _repo.InsertAsync(template);
        return MapToDto(saved);
    }

    // method to compaure the schema defined in the template and the modifications passed in by the user
    public async Task<SheetValidationResult> ValidateSheetAsync(Guid templateId, string sheetBlob)
    {
        int num;
        bool correctType;
        var template = await _repo.GetByIdAsync(templateId);
        SheetValidationResult result = new();

        if(template == null)
        {
            throw new Exception($"Template {templateId} not found");
        }

        List<SchemaDefinition> fields = JsonSerializer.Deserialize<List<SchemaDefinition>>(template.JsonSchema) ?? new();

        Dictionary<string, object> sheetFields = JsonSerializer.Deserialize<Dictionary<string,object>>(sheetBlob) ?? new();

        foreach (SchemaDefinition field in fields)
        {
            if (field.required && !sheetFields.ContainsKey(field.name))
            {
                ValidationError err = new ValidationError
                {
                    Field = field.name,
                    Message = $"{field.name} not found in schema."
                };
                result.Errors.Add(err);
            }
            else if (sheetFields.ContainsKey(field.name))
            {
                var holder = sheetFields[field.name].ToString();

                if (field.type == "Number")
                {
                    correctType = int.TryParse(holder, out num);
                    if (correctType == false)
                    {
                        ValidationError err = new ValidationError
                        {
                            Field = field.name,
                            Message = $"{field.name} is of wrong type or indiscernable."
                        };
                        result.Errors.Add(err);
                        break;
                    }
                    continue;
                }
            }
        }

        return result;
    }


    private static TemplateDto MapToDto(Template t)
    {
        return new()
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

    enum Types
    {
        Number,
        Text
    }
}