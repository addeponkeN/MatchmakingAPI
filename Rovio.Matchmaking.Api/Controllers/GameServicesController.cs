using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Api.Repositories;
using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api.Controllers;

/// <summary>
/// Handles out tokens to clients
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class GameServicesController : ControllerBase
{
    private IClientRepository _repository;

    public GameServicesController(IClientRepository repo)
    {
        _repository = repo;
    }

    /// <summary>
    /// Returns a token that is needed for the Matchmaking API calls.
    /// </summary>
    /// <param name="server">ServerModel containing a </param>
    /// <returns>A model containing the Game Service Id and the token that is required to make any Matchmaking API calls</returns>
    [HttpPost("register/{server}")]
    public ActionResult<ValidatedServerModel> RegisterGame(ServerModel server)
    {
        //  Check and see if the Game Service Id is valid
        var success = _repository.TryRegisterServer(server, out var validatedServer);

        if(!success)
        {
            return Problem(detail: "Invalid Game Service Id");
        }

        return Ok(validatedServer);
    }
}