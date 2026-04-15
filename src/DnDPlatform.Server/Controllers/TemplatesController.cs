using DnDPlatform.Models.DTOs.Templates;
using DnDPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

[ApiController]
[Route("api/templates")]
[Authorize]
public class TemplatesController(ITemplateService templateService) : AuthorizedControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateDto>>> GetAll([FromQuery] string? system) =>
        Ok(await templateService.GetTemplatesAsync(system));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TemplateDto>> GetById(Guid id)
    {
        try
        {
            return Ok(await templateService.GetTemplateAsync(id));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost]
    public async Task<ActionResult<TemplateDto>> Create(CreateTemplateRequest request)
    {
        try
        {
            var template = await templateService.CreateTemplateAsync(CurrentUserId, request);
            return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
