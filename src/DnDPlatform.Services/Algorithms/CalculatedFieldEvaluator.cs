using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DnDPlatform.Services.Algorithms;

public static class CalculatedFieldEvaluator
{
    private static readonly Regex TokenPattern = new(@"\b([a-zA-Z_][a-zA-Z0-9_]*)\b");
    public static string Evaluate(string templateSchema, string sheetBlob)
    {
        JsonDocument schemaDoc;
        JsonDocument sheetDoc;

        try
        {
            schemaDoc = JsonDocument.Parse(templateSchema);
            sheetDoc = JsonDocument.Parse(sheetBlob);
        }
        catch
        {
            return sheetBlob;
        }

        using (schemaDoc)
        using (sheetDoc)
        {
            if (!schemaDoc.RootElement.TryGetProperty("fields", out var fields))
            {
                return sheetBlob;
                
            }

            var values = ExtractValues(sheetDoc.RootElement);
            var calculatedFields = new Dictionary<string, string>();

            foreach (var field in fields.EnumerateArray())
            {
                if (!field.TryGetProperty("formula", out var formula))
                {
                    continue;      
                }
                if (!field.TryGetProperty("key", out var key))
                {
                    continue;
                    
                }

                var formulaStr = formula.GetString() ?? string.Empty;
                var keyStr = key.GetString() ?? string.Empty;
                calculatedFields[keyStr] = formulaStr;
            }

            if (calculatedFields.Count == 0)
            {
                return sheetBlob;    
            }

            // used to resolve dependencies in order
            var sortedKeys = TopologicalSort(calculatedFields);

            foreach (var fieldKey in sortedKeys)
            {
                var formula = calculatedFields[fieldKey];
                var resolved = ResolveFormula(formula, values);
                if (double.TryParse(resolved, out var num)) values[fieldKey] = num;
            }

            // rebuild JSON with calculated values added in
            var dict = new Dictionary<string, object>();
            foreach (var prop in sheetDoc.RootElement.EnumerateObject())
            {
                dict[prop.Name] = GetValue(prop.Value);       
            }

            foreach (var (k, v) in values)
            {
                dict[k] = v;     
            }

            return JsonSerializer.Serialize(dict);
        }
    }

    private static Dictionary<string, double> ExtractValues(JsonElement root)
    {
        var values = new Dictionary<string, double>();
        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.Number)
            {
                values[prop.Name] = prop.Value.GetDouble();
            }
        }
        return values;
    }

    private static string ResolveFormula(string formula, Dictionary<string, double> values)
    {
        var resolved = TokenPattern.Replace(formula, m =>
        {
            var token = m.Value;
            return values.TryGetValue(token, out var val) ? val.ToString() : "0";
        });

        try
        {
            var dt = new DataTable();
            var result = dt.Compute(resolved, null);
            return Convert.ToDouble(result).ToString();
        }
        catch
        {
            return "0";
        }
    }

    private static List<string> TopologicalSort(Dictionary<string, string> formulas)
    {
        var sorted = new List<string>();
        var visited = new HashSet<string>();

        void Visit(string key)
        {
            if (visited.Contains(key)) return;
            visited.Add(key);

            if (formulas.TryGetValue(key, out var formula))
            {
                foreach (Match m in TokenPattern.Matches(formula))
                {
                    if (formulas.ContainsKey(m.Value))
                        Visit(m.Value);
                }
            }

            sorted.Add(key);
        }

        foreach (var key in formulas.Keys)
        {
            Visit(key);
        }

        return sorted;
    }

    private static object GetValue(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Number => element.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => (object)"",
        _ => element.GetString() ?? ""
    };
}
