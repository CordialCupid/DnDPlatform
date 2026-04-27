using DnDPlatform.Models;
using DnDPlatform.Models.Domain;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DnDPlatform.Services.Algorithms;

public static class SheetValidationEngine
{
    public static SheetValidationResult Validate(Template template, string sheetBlob)
    {
        var result = new SheetValidationResult();

        // attempt to convert passed in json sheet data and parse into json
        JsonDocument? sheetDoc;
        try
        {
            sheetDoc = JsonDocument.Parse(sheetBlob);
        }
        catch
        {
            result.Errors.Add(new ValidationError { Field = "Root of the json blob", Message = "Sheet blob is not valid JSON" });
            return result;
        }

        // try to parse the json schema of the given template 
        JsonDocument? schemaDoc;
        try
        {
            schemaDoc = JsonDocument.Parse(template.JsonSchema);
        }
        catch
        {
            return result; 
        }

        // this next section is intended to compare the passed in sheet blobl and its fields/properties against the tempaltes json schema for validation
        using (sheetDoc)
        using (schemaDoc)
        {
            // looks for the fields property in the schema json document, this holds the validation for fields that the user passes in to actually define a template for their character
            // returns the empty validaiton result object because if the schema has no fields property defined, then there is nothing to compare the sheet against
            if (!schemaDoc.RootElement.TryGetProperty("fields", out var fields))
            {
                return result;          
            }

            // start at the outermost level of the sheet json 
            var sheetRoot = sheetDoc.RootElement;

            // start looping through each field in the fields element in the json
            foreach (var field in fields.EnumerateArray())
            {
                // retrieves key property 
                if (!field.TryGetProperty("key", out var keyProp))
                {
                    continue;  
                }


                var key = keyProp.GetString() ?? string.Empty;
                var required = field.TryGetProperty("required", out var req) && req.GetBoolean();
                var dataType = field.TryGetProperty("dataType", out var dt) ? dt.GetString() ?? "string" : "string";

                if (field.TryGetProperty("condition", out var condition))
                {
                    if (!EvaluateCondition(condition, sheetRoot))
                    {
                        continue;        
                    }
                }

                var hasValue = sheetRoot.TryGetProperty(key, out var value);

                if (required && (!hasValue || IsEmpty(value)))
                {
                    result.Errors.Add(new ValidationError { Field = key, Message = $"Field '{key}' is required." });
                    continue;
                }

                if (!hasValue || IsEmpty(value))
                {
                    continue;            
                }

                var typeError = ValidateType(key, value, dataType);
                if (typeError is not null)
                {
                    result.Errors.Add(typeError);
                    continue;
                }

                if (dataType == "number" || dataType == "integer")
                {
                    var numVal = value.GetDouble();
                    if (field.TryGetProperty("min", out var min) && numVal < min.GetDouble())
                    {
                        result.Errors.Add(new ValidationError { Field = key, Message = $"Field '{key}' must be >= {min.GetDouble()}." });
                        
                    }
                    if (field.TryGetProperty("max", out var max) && numVal > max.GetDouble())
                    {
                        result.Errors.Add(new ValidationError { Field = key, Message = $"Field '{key}' must be <= {max.GetDouble()}." });
                    }
                }

                if (dataType == "string" && field.TryGetProperty("pattern", out var pattern))
                {
                    var strVal = value.GetString() ?? string.Empty;
                    var patternStr = pattern.GetString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(patternStr) && !Regex.IsMatch(strVal, patternStr))
                    {
                        result.Errors.Add(new ValidationError { Field = key, Message = $"Field '{key}' does not match the required pattern." });
                    }
                }
            }
        }

        return result;
    }

    private static bool IsEmpty(JsonElement value) =>
        value.ValueKind == JsonValueKind.Null ||
        (value.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(value.GetString()));

    private static ValidationError? ValidateType(string key, JsonElement value, string dataType)
    {
        return dataType switch
        {
            "number" or "integer" when value.ValueKind != JsonValueKind.Number =>
                new ValidationError { Field = key, Message = $"Field '{key}' must be a number." },
            "boolean" when value.ValueKind != JsonValueKind.True && value.ValueKind != JsonValueKind.False =>
                new ValidationError { Field = key, Message = $"Field '{key}' must be a boolean." },
            "string" when value.ValueKind != JsonValueKind.String =>
                new ValidationError { Field = key, Message = $"Field '{key}' must be a string." },
            _ => null
        };
    }

    // method to return a bool indicating if 
    private static bool EvaluateCondition(JsonElement condition, JsonElement sheetRoot)
    {
        if (!condition.TryGetProperty("field", out var fieldProp) || !condition.TryGetProperty("value", out var valueProp))
        {
            return true;    
        }

        var condField = fieldProp.GetString() ?? string.Empty;
        if (!sheetRoot.TryGetProperty(condField, out var actual))
        {

            
            return false;
            
        }

        return actual.ToString() == valueProp.ToString();
    }
}
