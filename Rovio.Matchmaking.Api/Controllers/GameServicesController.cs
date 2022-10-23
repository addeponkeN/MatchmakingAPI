using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Api.Repositories;
using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GameServicesController : ControllerBase
{
    private IClientRepository _repository;
    
    public GameServicesController(IClientRepository repo)
    {
        _repository = repo;
    }
    
    [HttpPost("register/{server}")]
    public ActionResult<ValidatedServerModel> RegisterGame(ServerModel server)
    {
        var success = _repository.TryRegisterServer(server, out var validatedServer);

        if(!success)
            return Problem(detail: "Invalid Game Service Id");
        
        return Ok(validatedServer);
    }
}