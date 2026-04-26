using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : AuthorizedControllerBase
{
    // Endpoint for getting the current users id and username
    [HttpGet]
    public IActionResult GetMe() 
    {
        return Ok(new
        {
            UserId = CurrentUserId,
            Username = CurrentUsername
        });
    }
}
