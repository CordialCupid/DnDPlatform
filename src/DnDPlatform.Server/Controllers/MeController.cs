using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : AuthorizedControllerBase
{
    [HttpGet]
    public IActionResult GetMe() => Ok(new
    {
        UserId = CurrentUserId,
        Username = CurrentUsername
    });
}
