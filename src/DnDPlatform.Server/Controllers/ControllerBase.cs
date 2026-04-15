using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

public abstract class AuthorizedControllerBase : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("sub")
                   ?? throw new UnauthorizedAccessException("No user identity in token.");
            return Guid.Parse(sub);
        }
    }

    protected string CurrentUsername =>
        User.FindFirstValue(ClaimTypes.Name)
     ?? User.FindFirstValue("unique_name")
     ?? "unknown";
}
