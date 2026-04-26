using DnDPlatform.Models.DTOs.Templates;
using DnDPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

[ApiController]
[Route("api/templates")]
[Authorize]
public class TemplatesController : AuthorizedControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplatesController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    // endpoint to get all templates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateDto>>> GetAll([FromQuery] string? system)
    {
        
        return Ok(await _templateService.GetTemplatesAsync(system));
    }


    // endpoint to retireve specific templates by id
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TemplateDto>> GetById(Guid id)
    {
        try
        {
            return Ok(await _templateService.GetTemplateAsync(id));
        }
        catch (KeyNotFoundException ex) 
        { 
            return NotFound(new { message = ex.Message }); 
        }
    }

    // endpoint to create a template
    [HttpPost]
    public async Task<ActionResult<TemplateDto>> Create(CreateTemplateRequest request)
    {
        try
        {
            var template = await _templateService.CreateTemplateAsync(CurrentUserId, request);
            
            return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
