using DnDPlatform.Models.DTOs.DnD5e;
using DnDPlatform.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace DnDPlatform.Services.Implementations;

public class DnDInfoService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    ILogger<DnDInfoService> logger) : IDnDInfoService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);
    private const string BaseUrl = "https://www.dnd5eapi.co/api";

    public Task<IEnumerable<DnDClassDto>> GetClassesAsync() =>
        GetCachedAsync("dnd5e_classes", FetchClassesAsync);

    public Task<IEnumerable<DnDSpellDto>> GetSpellsAsync(string? filter = null)
    {
        var key = string.IsNullOrWhiteSpace(filter) ? "dnd5e_spells" : $"dnd5e_spells_{filter}";
        return GetCachedAsync(key, () => FetchSpellsAsync(filter));
    }

    public Task<IEnumerable<DnDEquipmentDto>> GetEquipmentAsync() =>
        GetCachedAsync("dnd5e_equipment", FetchEquipmentAsync);

    public Task<IEnumerable<DnDAbilityScoreDto>> GetAbilityScoresAsync() =>
        GetCachedAsync("dnd5e_ability_scores", FetchAbilityScoresAsync);

    private async Task<IEnumerable<T>> GetCachedAsync<T>(string key, Func<Task<IEnumerable<T>>> fetch)
    {
        if (cache.TryGetValue(key, out IEnumerable<T>? cached) && cached is not null)
            return cached;

        var data = await fetch();
        cache.Set(key, data, CacheTtl);
        return data;
    }

    private async Task<IEnumerable<DnDClassDto>> FetchClassesAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("DnD5e");
            var resp = await client.GetFromJsonAsync<DnD5eListResponse>($"{BaseUrl}/classes");
            return resp?.Results?.Select(r => new DnDClassDto { Index = r.Index, Name = r.Name, Url = r.Url }) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch D&D 5e classes");
            return [];
        }
    }

    private async Task<IEnumerable<DnDSpellDto>> FetchSpellsAsync(string? filter)
    {
        try
        {
            var client = httpClientFactory.CreateClient("DnD5e");
            var url = string.IsNullOrWhiteSpace(filter) ? $"{BaseUrl}/spells" : $"{BaseUrl}/spells?name={Uri.EscapeDataString(filter)}";
            var resp = await client.GetFromJsonAsync<DnD5eListResponse>(url);
            return resp?.Results?.Select(r => new DnDSpellDto { Index = r.Index, Name = r.Name, Url = r.Url }) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch D&D 5e spells");
            return [];
        }
    }

    private async Task<IEnumerable<DnDEquipmentDto>> FetchEquipmentAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("DnD5e");
            var resp = await client.GetFromJsonAsync<DnD5eListResponse>($"{BaseUrl}/equipment");
            return resp?.Results?.Select(r => new DnDEquipmentDto { Index = r.Index, Name = r.Name, Url = r.Url }) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch D&D 5e equipment");
            return [];
        }
    }

    private async Task<IEnumerable<DnDAbilityScoreDto>> FetchAbilityScoresAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient("DnD5e");
            var resp = await client.GetFromJsonAsync<DnD5eListResponse>($"{BaseUrl}/ability-scores");
            return resp?.Results?.Select(r => new DnDAbilityScoreDto { Index = r.Index, Name = r.Name, Url = r.Url }) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to fetch D&D 5e ability scores");
            return [];
        }
    }

    private class DnD5eListResponse
    {
        public int Count { get; set; }
        public List<DnD5eResultItem>? Results { get; set; }
    }

    private class DnD5eResultItem
    {
        public string Index { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
