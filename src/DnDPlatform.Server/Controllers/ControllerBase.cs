using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DnDPlatform.Server.Controllers;

// new controller base to support authorization for controllers that require it
public abstract class AuthorizedControllerBase : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                var nullId = User.FindFirstValue("sub");
                if (nullId == null)
                {
                    throw new UnauthorizedAccessException("No user identity in token.");
                }
            }
            return Guid.Parse(id);
        }
    }

    protected string CurrentUsername
    {
        get
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username == null)
            {
                var nullUsername = User.FindFirstValue("unique_name");
                if (nullUsername == null)
                {
                    username = "unknown";
                }
            }
            return username;
            
        }
    }
        
}
