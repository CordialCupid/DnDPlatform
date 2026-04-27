using DnDPlatform.Models;
using DnDPlatform.Models.DTOs.Characters;
using DnDPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

[ApiController]
[Route("api/characters")]
[Authorize]
public class CharactersController : AuthorizedControllerBase
{
    private readonly ICharacterService _characterService;
    private readonly ICharacterCreationService _creationService;
    private readonly IVersionSnapshotManager _versionManager;
    private readonly ITemplateService _templateService;

    public CharactersController(ICharacterService characterService,ICharacterCreationService creationService, IVersionSnapshotManager versionManager, ITemplateService templateService)
    {
        _characterService = characterService;
        _creationService = creationService;
        _versionManager = versionManager;
        _templateService = templateService;
    }

    // endpoint for retrieving all your character sheets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> GetAll()
    {
        return Ok(await _characterService.GetCharactersAsync(CurrentUserId));
    }

    // POST endpoint for creating a new character
    [HttpPost]
    public async Task<ActionResult<CharacterDto>> Create(CreateCharacterRequest request)
    {
        var character = await _creationService.CreateCharacterAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = character.Id }, character);
    }

    // GET endpoint for retrieving a character by a specific ID
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CharacterDto>> GetById(Guid id)
    {
        return Ok(await _characterService.GetCharacterAsync(CurrentUserId, id));
    }

    // Endpoint for saving a characters sheet
    [HttpPut("{id:guid}/sheet")]
    public async Task<ActionResult<SheetVersionDto>> SaveSheet(Guid id, SaveSheetRequest request)
    {
        var character = await _characterService.GetCharacterAsync(CurrentUserId, id);
        
        // validate whether the save sheet request contains values for the json schema defined in the template
        // e.g. if the template defined strength and it was required, then the charcater sheet save must have that field in there 
        var validation = await _templateService.ValidateSheetAsync(character.TemplateId, request.sheetData);
        if (!validation.IsValid)
        {
            return UnprocessableEntity(validation.Errors);   
        }

        var version = await _versionManager.SaveCurrentAsync(CurrentUserId, id, request.sheetData);

        return Ok(version);
    }

    // Endpoint for creating a new snapshot of a character sheet
    [HttpPost("{id:guid}/snapshot")]
    public async Task<ActionResult<SheetVersionDto>> CreateSnapshot(Guid id, CreateSnapshotRequest request)
    {

        var character = await _characterService.GetCharacterAsync(CurrentUserId, id);
        var current = character.CurrentSheet;
        if (current == null)
        {
            throw new InvalidOperationException("No sheet data to snapshot.");
        }

        var snapshot = await _versionManager.CreateSnapshotAsync(CurrentUserId, id, current.JsonBlob, request.Label);
        return Ok(snapshot);
    }

    // Endpoint for getting the versions of a character
    [HttpGet("{id:guid}/versions")]
    public async Task<ActionResult<IEnumerable<VersionSummaryDto>>> GetVersions(Guid id)
    {
        return Ok(await _versionManager.GetVersionHistoryAsync(CurrentUserId, id));
    }

    // Delete character endpoint
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _characterService.DeleteCharacterAsync(CurrentUserId, id);
        return NoContent();
    }
}
