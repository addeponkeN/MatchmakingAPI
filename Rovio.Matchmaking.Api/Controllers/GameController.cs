using Microsoft.AspNetCore.Mvc;

namespace Rovio.Matchmaking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GameController : ControllerBase
{
    [HttpPost]
    public Task<Guid> RegisterGame(string nameId)
    {
        return null;
    }
}