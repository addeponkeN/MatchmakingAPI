using Microsoft.AspNetCore.Mvc;
using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : Controller
{
    // GET api/Players/id
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return $"user #{id}";
    }

    // GET api/Players
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] {"pelle", "olle", "johnny"};
    }

    [HttpPost("{player}")]
    public bool Post(MatchmakePlayer player)
    {
        return true;
    }
}