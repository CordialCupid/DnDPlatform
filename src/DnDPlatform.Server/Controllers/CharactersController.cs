using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

[ApiController]
[Route("api/characters")]
[Authorize]
public class CharactersController(
    ICharacterService characterService,
    ICharacterCreationService creationService,
    IVersionSnapshotManager versionManager,
    ITemplateService templateService) : AuthorizedControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> GetAll() =>
        Ok(await characterService.GetCharactersAsync(CurrentUserId));

    [HttpPost]
    public async Task<ActionResult<CharacterDto>> Create(CreateCharacterRequest request)
    {
        try
        {
            var character = await creationService.CreateCharacterAsync(CurrentUserId, request);
            return CreatedAtAction(nameof(GetById), new { id = character.Id }, character);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CharacterDto>> GetById(Guid id)
    {
        try
        {
            return Ok(await characterService.GetCharacterAsync(CurrentUserId, id));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPut("{id:guid}/sheet")]
    public async Task<ActionResult<SheetVersionDto>> SaveSheet(Guid id, SaveSheetRequest request)
    {
        try
        {
            // Run validation before saving
            var character = await characterService.GetCharacterAsync(CurrentUserId, id);
            var validation = await templateService.ValidateSheetAsync(character.TemplateId, request.SheetData);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors);

            var version = await versionManager.SaveCurrentAsync(CurrentUserId, id, request.SheetData);
            return Ok(version);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{id:guid}/snapshot")]
    public async Task<ActionResult<SheetVersionDto>> CreateSnapshot(Guid id, CreateSnapshotRequest request)
    {
        try
        {
            var character = await characterService.GetCharacterAsync(CurrentUserId, id);
            var current = character.CurrentSheet ?? throw new InvalidOperationException("No sheet data to snapshot.");
            var snapshot = await versionManager.CreateSnapshotAsync(CurrentUserId, id, current.JsonBlob, request.Label);
            return Ok(snapshot);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id:guid}/versions")]
    public async Task<ActionResult<IEnumerable<VersionSummaryDto>>> GetVersions(Guid id)
    {
        try
        {
            return Ok(await versionManager.GetVersionHistoryAsync(CurrentUserId, id));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("{id:guid}/revert/{version:int}")]
    public async Task<ActionResult<SheetVersionDto>> Revert(Guid id, int version)
    {
        try
        {
            return Ok(await versionManager.RevertAsync(CurrentUserId, id, version));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpGet("{id:guid}/export")]
    public async Task<IActionResult> Export(Guid id)
    {
        try
        {
            var pdf = await characterService.ExportPdfAsync(CurrentUserId, id);
            return File(pdf, "text/plain", $"character_{id}.txt");
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await characterService.DeleteCharacterAsync(CurrentUserId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }
}
