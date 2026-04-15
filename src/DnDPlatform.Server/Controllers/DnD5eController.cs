using DnDPlatform.Models.DTOs.DnD5e;
using DnDPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

[ApiController]
[Route("api/dnd5e")]
[Authorize]
public class DnD5eController(IDnDInfoService dndInfoService) : ControllerBase
{
    [HttpGet("classes")]
    public async Task<ActionResult<IEnumerable<DnDClassDto>>> GetClasses() =>
        Ok(await dndInfoService.GetClassesAsync());

    [HttpGet("spells")]
    public async Task<ActionResult<IEnumerable<DnDSpellDto>>> GetSpells([FromQuery] string? filter) =>
        Ok(await dndInfoService.GetSpellsAsync(filter));

    [HttpGet("equipment")]
    public async Task<ActionResult<IEnumerable<DnDEquipmentDto>>> GetEquipment() =>
        Ok(await dndInfoService.GetEquipmentAsync());

    [HttpGet("ability-scores")]
    public async Task<ActionResult<IEnumerable<DnDAbilityScoreDto>>> GetAbilityScores() =>
        Ok(await dndInfoService.GetAbilityScoresAsync());
}
